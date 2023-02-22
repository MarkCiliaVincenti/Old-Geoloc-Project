using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
//using TeltonikaEmulator.Encoding;
using TeltonikaEmulator.Models;
using TeltonikaEmulator.TcpClient;

namespace TeltonikaEmulator
{
    public delegate void UpdateStartBtn(bool completed);

    public  class Emulator
    {
        public  UpdateLogDataGird UpdateLogDataGird;
        public  UpdateStartBtn UpdateStartBtn;
       
        static String[] IMEIs = new String[100]{
            /*"123456789012345","123456789012345","123456789012345","123456789012345","123456789012345","123456789012345","123456789012345","123456789012345","123456789012345","123456789012345",*/
            "356173060307977","356173060968455","356173060305146","356173066531356","356307049444762","356173062198986","356173064102267","356173062199299","356173065042694","356173065835030",
            "356173067293402","356173060499493","356173068565220","356173065047669","356173063827013","356173068201974","356173068701221","356173066538393","356173068201669","356173063371293",
            "356173068700959","356173064848562","356173061818444","356173068367171","356173068588412","356173061379652","356173065099413","356173068574297","356173068365621","356173064175123",
            "356173065981735","356173066413217","356173066518759","356173068564389","356173067287297","356173067199708","356173067268750","356173065034626","356173067267422","356173068366934",
            "356173060814832","356173061370115","356173065049020","356173067031000","356173066538773","356173066014858","356173061866781","356173067193347","356173067268321","356173060304123",
            "356173060853228","356173066014320","356173064066587","356173068568877","356173064038446","356173068110290","356173066541066","356173068567119","356173067260773","356173066537551",
            "356173068581854","356173068377212","356173067266838","356173067288436","356173067266788","356173060821548","356173065856168","356173064239416","356173068586390","356173066538815",
            "356173066537536","356173067262282","356173064132363","356173067287487","356173067319785","356173067266598","356173067195094","356173066529475","356173060968455","356173068110431",
            "356173063374313","356173068588909","356173067195417","356173067288295","356173067285838","356173067261078","356173068588214","356173066517553","356173068365597","356173067289244",
            "356173068694004","356173061869652","356173061406877","356173063352616","356173067905211","356173067286711","356173067197066","356173065437753","356173068562128","356173066412243"};

        public   void  Emulate(EmulationConfig config, List<Encoding.EncodedAvlData> encodedData, CancellationToken token )
        {
            switch (config.SourceOfImeIs)
            {
                case SourceOfIMEIs.FromLocalList:
                    config.IMEIs = IMEIs;
                    if (config.BoxNumber > 100)
                        config.BoxNumber = 100;
                    break;
                case SourceOfIMEIs.FromDatabase:
                    try
                    {
                        GetLog("Requete pour ramener la liste des  IMEIs a partir de la base de données ... ", LogType.Info);
                    
                    }
                    catch (Exception e)
                    {
                        GetLog("Il ya un problème quelque part avec la base de données :" +  e.Message, LogType.Error);

                        Console.WriteLine(e);
                        return;
                        // throw;
                    }
                    break;
                case SourceOfIMEIs.OneIMEI:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Parallel.For((long)0, config.BoxNumber, async (index, state) =>
            {
                try
                {
                    var i1 = index;
                    foreach (var data in encodedData.GroupBy(x => x.SocketNumber))
                    {
                        try
                        {
                            token.ThrowIfCancellationRequested();
                            await RunEmulationAsync(config,config.IMEIs[ i1], data, token).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException e)
                        {
                            Console.WriteLine(e.Message);
                            UpdateStartBtn?.Invoke(false);
                            state.Stop();
                            throw;
                        }
                    }
                }
                catch (Exception e)
                {
                    GetLog(e.Message, LogType.Error);
                }
            });

        }

        private async Task RunEmulationAsync(EmulationConfig config, string i1, IGrouping<string, Encoding.EncodedAvlData> data, CancellationToken token)
        {

            // initialiser le client
            var _client = new Client(config.IpAddress, config.Port);
            _client.OnDisconnected += (obj, args) =>
            {
                GetLog($"Le serveur {config.IpAddress}:{config.Port} a été déconnecté...", LogType.Error);
                //return;
            };

            try
            {
                // tentative de connection au serveur
                await _client.ConnectAsync(token).ConfigureAwait(false);
                _client.IsConnected = true;
                GetLog($"Etablissement de la connexion au serveur  {config.IpAddress}:{config.Port} ...", LogType.Info);
            }
            catch (Exception e)
            {
                GetLog(e.Message, LogType.Error);
                return;
            }
            // authentification par l'envoi de l'IMEI
            Byte[] imei = BitConverter.GetBytes((ushort)15).Reverse().Concat(System.Text.Encoding.ASCII.GetBytes(i1)).ToArray();

            await _client.SendAsync(imei, token).ConfigureAwait(false);

            var bt = await _client.ReceiveAsync(token).ConfigureAwait(false);
            if (bt[0] == 0x001)
            {
                GetLog($"Envoie de {data.Count()} trames  IEMI :{i1} ... ", LogType.Info);
                // envoi des trames AVL
                foreach (var encodedAvlData in data)
                {
                    if (token.IsCancellationRequested)
                        return;

                    await _client.SendAsync(encodedAvlData.Data, token).ConfigureAwait(false);
                    var aqt = await _client.ReceiveAsync(token).ConfigureAwait(false);
                    Console.WriteLine(aqt[3]);
                    Thread.Sleep((int)config.SleepPeriod);
                }
                _client.CloseStream();
                _client.Disconnect();
            }

        }
        private  void GetLog(string message, LogType type)
        {
            UpdateLogDataGird?.Invoke(new LogVm
            {
                Date = DateTime.Now,
                Type = type,
                Description = message
            });
        }
    }

    public class EmulationConfig
    {
        public int BoxNumber { get; set; }
        public string[] IMEIs { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public decimal SleepPeriod { get; set; }
        public  SourceOfIMEIs SourceOfImeIs { get; set; }
       // public string Imei { get; set; }


    }

    public enum SourceOfIMEIs
    {
        OneIMEI,
        FromDatabase,
        FromLocalList
    }
}
