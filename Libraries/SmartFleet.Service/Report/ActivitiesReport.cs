using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SmartFleet.Core.Domain.Movement;
using SmartFleet.Core.Geofence;
using SmartFleet.Service.Models;

namespace SmartFleet.Service.Report
{
    /// <summary>
    /// 
    /// </summary>


    public delegate void UpdateProgress(int val);

    public class ActivitiesReport : IActivitiesReport
    {
        /// <summary>
        /// 
        /// </summary>
        public UpdateProgress UpdateProgress { get; set; }  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="positionsOfVehicle"></param>
        /// <returns></returns>
        public  List<PositionViewModel> PositionViewModels(List<Position> positionsOfVehicle )
        {
            return positionsOfVehicle.Select(p => new PositionViewModel(p, p.Vehicle)).ToList();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="startPeriod"></param>
        /// <param name="vehicleName"></param>
        /// <returns></returns>
        public List<TargetViewModel> BuildDailyReport(List<Position> positions, DateTime startPeriod, string vehicleName)
        { 
            // ReSharper disable once TooManyChainedReferences
            var firstPos = positions.OrderBy(p => p.Timestamp).ThenBy(p => p.MotionStatus).FirstOrDefault();
            var currentStatus =firstPos ?.MotionStatus;
            var start = positions.FirstOrDefault()?.Timestamp;
            var periods = new List<Periods>();
            var orderedEnumerable = positions.OrderBy(p => p.Timestamp).ThenBy(p=>p.MotionStatus);
            // ReSharper disable once PossibleMultipleEnumeration
            var motionStatusCount = orderedEnumerable.GroupBy(x => x.MotionStatus).Count();
            var motionStatusChanged = false;
            // rearrange periods by date / status of driving on 24 hour scale
            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var position in orderedEnumerable)
                motionStatusChanged = MotionStatusChanged(position, periods, ref currentStatus, ref start);
            // check if there is only one period or the last period is missing  to add both to the periods list 
            // ReSharper disable once ComplexConditionExpression
            if (motionStatusCount == 1 || !motionStatusChanged)
            {
                if (motionStatusCount == 1)
                {
                    // if theres is only one period we add it to the list
                    // ReSharper disable once PossibleMultipleEnumeration
                    // ReSharper disable once PossibleNullReferenceException
                    var poriod = new Periods(orderedEnumerable.LastOrDefault().Timestamp, start, currentStatus);
                    periods.Add(poriod);
                }
                else if (!motionStatusChanged)
                {
                    // ReSharper disable once PossibleMultipleEnumeration
                    var item = GetLastPeriods(periods, orderedEnumerable);
                    periods.Add(item);
                }
            }
            var tmp = new List<Periods>();
            // get rid of the periods with duration less than 60 sec
            GetRidOfShortPeriods(periods, tmp);
            // create the final report
            return BuildReport(positions, startPeriod, vehicleName, periods);
        }

        private static void ReMergePeriods(List<Periods> periods, List<Periods> tmp)
        {
            foreach (var period in periods)
            {
                var index = periods.FindIndex(p => p == period);
                // ReSharper disable once ComplexConditionExpression
                if (index + 1 >= periods.Count || period.MotionStatus != periods[index + 1].MotionStatus) continue;
                // ReSharper disable once ComplexConditionExpression
                while (index + 1 < periods.Count && period.MotionStatus == periods[index + 1].MotionStatus)
                {
                    period.End = periods[index + 1].End;
                    tmp.Add(periods[index + 1]);
                    index++;
                }
            }
            // get rid of the periods have been merged

            DeleteTempPeriods(tmp, periods);


        }

        private static void GetRidOfShortPeriods(List<Periods> periods, List<Periods> tmp)
        {
            foreach (var period in periods)
                if ((period.End - period.Start).TotalSeconds < 60)
                    tmp.Add(period);
            DeleteTempPeriods(tmp, periods);
            tmp = new List<Periods>();
            // re merge periods after removing short periods
            ReMergePeriods(periods, tmp);
        }

        private static Periods GetLastPeriods(List<Periods> periods, IOrderedEnumerable<Position> orderedEnumerable)
        {
            var end = periods.LastOrDefault()?.End;
            var query = orderedEnumerable.Where(x => x.Timestamp >= end);
            var enumerable
                = query as Position[] ?? query.ToArray();
            var lastPeriods = new Periods(enumerable.LastOrDefault()?.Timestamp, end, enumerable.LastOrDefault()?.MotionStatus);
            return lastPeriods;
        }

        // ReSharper disable once MethodTooLong
        // ReSharper disable once TooManyArguments
        public List<TargetViewModel> BuildReport(List<Position> positions, DateTime startPeriod, string vehicleName, List<Periods> periods)
        {
            List<TargetViewModel> targets = new List<TargetViewModel>();
            var i = 50;
            var max = periods.Count;
            foreach (var position in periods.Distinct())
            {
                if (i < 100) i += 100 / max;
                else i = 99;
                UpdateProgress?.Invoke(i);
                var trgt = new TargetViewModel();
                // get the first period 
                var firstPosition = GetFirstPosition(positions, position);
                //get  last period
                var lastPosition = GetLastPosition(positions, position);
                // set the begnings of the period
                SetBeginningsTrip(firstPosition, trgt);
                // set the ends of the period
                SetEndsTrip(lastPosition, trgt);
                // calculate distance , speeds , and duration
                GetDistanceAndDuration(positions, trgt, position.Start, position.End);
                var index = GetPreviousPositionIndex(positions, lastPosition);
                if (index == -1)
                    index = 0;
                trgt.Speed = positions.ElementAt(index).Speed;
                trgt.VehicleName = vehicleName;
                trgt.CurrentDate = startPeriod.Date.ToShortDateString();
                targets.Add(trgt);
            }

            return targets;
        }

        private static void DeleteTempPeriods(List<Periods> tmp, List<Periods> periods)
        {
            foreach (var period in tmp)
                periods.Remove(period);
        }

        // ReSharper disable once TooManyArguments
        private static bool MotionStatusChanged(Position position, List<Periods> periods, ref MotionStatus? currentStatus,
            ref DateTime? start)
        {
            Trace.WriteLine("DateTimeUtc: " + position.Timestamp + " MotionStatus: " + position.MotionStatus);
            if (currentStatus == position.MotionStatus)
                return false;
            var item = new Periods(position.Timestamp, start, currentStatus);
            currentStatus = position.MotionStatus;
            start = position.Timestamp;
            periods.Add(item);
            Trace.WriteLine("period added at : " + position.Timestamp + " MotionStatus: " + position.MotionStatus);
            return true;
        }

        private static int GetPreviousPositionIndex(List<Position> positions, Position lastPosition)
        {
            int index = positions.ToList().FindIndex(x => x == lastPosition);
            return index -1;
        }

        private static Position GetLastPosition(List<Position> positions, Periods position)
        {
            var lastPosition = positions.LastOrDefault(x => x.Timestamp == position.End);
            return lastPosition;
        }

        // ReSharper disable once MethodTooLong
        // ReSharper disable once TooManyArguments
        private static void GetDistanceAndDuration(List<Position> positions, TargetViewModel trgt, DateTime currenPosition, DateTime positionEnd)
        {
            // ReSharper disable once ComplexConditionExpression
            var points = positions.Where(x => x.Timestamp >= currenPosition && x.Timestamp <= positionEnd).ToList();
            var avgSpeed = points.Average(x => x.Speed);
            trgt.AvgSpeed = Math.Round(avgSpeed, 2);
            trgt.MaxSpeed = Math.Round(points.Max(x => x.Speed), 2);

            trgt.Distance = 0;
            // ReSharper disable once ComplexConditionExpression
            if (points.Any() && trgt.MotionStatus!="Stopped")
            {
                var firstPos = points.First();
                foreach (var p in points.OrderBy(x=>x.Timestamp).Skip(1))
                {
                    var dis = Math.Round(GeofenceHelper.HaversineFormula(new GeofenceHelper.Position{ Latitude= firstPos.Lat,Longitude= firstPos.Long }, new GeofenceHelper.Position{Latitude = p.Lat,Longitude= p.Long }, GeofenceHelper.DistanceType.Kilometers),2);
                    if (!double.IsNaN(dis))
                        trgt.Distance += dis;
                    firstPos = p;
                }
            }

            if (double.IsNaN(trgt.Distance))
                trgt.Distance = 0;
            trgt.Distance = Math.Round(trgt.Distance, 2);
            trgt.Duration = (DateTime.Parse(trgt.EndPeriod) - DateTime.Parse(trgt.StartPeriod)).TotalSeconds;
        }

        private static Position GetFirstPosition(List<Position> positions, Periods position)
        {
            var firstPosition =
                // ReSharper disable once ComplexConditionExpression
                positions.FirstOrDefault(x => x.Timestamp == position.Start && x.MotionStatus == position.MotionStatus);
            return firstPosition;
        }

        private static void SetEndsTrip(Position lastPosition, TargetViewModel trgt)
        {
            if (lastPosition != null)
            {
                trgt.EndPeriod = lastPosition.Timestamp.ToString("O");
                trgt.EndPeriod1 = lastPosition.Timestamp.ToString("g");
                trgt.ArrivalAddres = lastPosition.Address;
            }
        }

        private static void SetBeginningsTrip(Position firstPosition, TargetViewModel trgt)
        {
            if (firstPosition == null) return;
            trgt.MotionStatus = firstPosition.MotionStatus.ToString();

            trgt.StartPeriod = firstPosition.Timestamp.ToString("O");
            trgt.StartPeriod1 = firstPosition.Timestamp.ToString("g");

            trgt.Latitude = firstPosition.Lat;
            trgt.Logitude = firstPosition.Long;
            trgt.StartAddres = firstPosition.Address;
            trgt.MotionStatus = firstPosition.MotionStatus.ToString();
        }

     
    }
}