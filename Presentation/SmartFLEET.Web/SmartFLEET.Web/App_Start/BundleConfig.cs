using System.Web.Optimization;

namespace SmartFLEET.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundles"></param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jq-1.10.2.js",
                "~/Scripts/angular.min.js",
               "~/Scripts/angular-route.min.js",
                "~/Scripts/angular-ui-router.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            // Utilisez la version de développement de Modernizr pour le développement et l'apprentissage. Puis, une fois
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap3.min.js",
                "~/Scripts/moment-js/moment.js",
                "~/Scripts/jq-layout.js",
                "~/Scripts/leafjs/leaflet.js",
                "~/Scripts/jstree/jstree.min.js",
                "~/Scripts/jquery.signalR-2.3.0.min.js",
                "~/Scripts/datatables/js/jquery.dataTables.js",
                "~/Scripts/datatables/js/dataTables.bootstrap.js",
                "~/Scripts/bootstrap-growl.js",
                "~/Scripts/timepicker/jquery.datetimepicker.full.min.js",
                "~/dist/plugins/select2/select2.full.min.js",
                "~/dist/plugins/loadie/js/jquery.loadie.min.js",
                "~/Scripts/jstree/jstree.min.js",
                "~/dist/plugins/vis/vis.min.js",
                "~/dist/plugins/leafletPlugin/PolylineDecorator.js",
                "~/Scripts/angularjs-gauge.js",
                "~/dist/plugins/wait/waitMe.min.js",
                "~/Scripts/jquery.stacky.js",
                "~/Scripts/easyui/jquery.easyui.min.js",
                "~/Scripts/app/mainApp.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/jsAdmin").Include(
               "~/Scripts/bootstrap3.min.js",
                "~/Scripts/datatables/js/jquery.dataTables.js",
                "~/Scripts/datatables/js/dataTables.bootstrap.js",
                "~/Scripts/bootstrap-growl.js",
                "~/dist/plugins/datepicker/bootstrap-datepicker.js",
                "~/dist/plugins/datepicker/locales/bootstrap-datepicker.fr.js",
                "~/dist/plugins/select2/select2.full.min.js"
               
            ));
            bundles.Add(new ScriptBundle("~/bundles/jsApp").Include(
                "~/dist/js/app.min.js"
                ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap3.css",
                "~/Content/validationEngine.jquery.css",
                "~/Content/font-awesome.css",
                "~/Scripts/datatables/css/dataTables.bootstrap.css",
                "~/Scripts/timepicker/jquery.datetimepicker.css",
                "~/dist/plugins/loadie/css/loadie.css",
                "~/Content/jquery-ui.min.css",
                "~/font-awesome/css/font-awesome.min.css",
                "~/Scripts/jstree/themes/default/style.min.css",
                "~/dist/plugins/vis/vis.min.css",
                "~/Scripts/jstree/themes/default/style.min.css",
                "~/dist/plugins/wait/waitMe.min.css",
                "~/dist/plugins/line-awesome/css/line-awesome.min.css"
               
                ));
            bundles.Add(new StyleBundle("~/Content/cssadmin").Include(
                "~/Content/bootstrap3.css",
                "~/Content/validationEngine.jquery.css",
                "~/Scripts/datatables/css/dataTables.bootstrap.css",
                "~/Content/font-awesome.css",
                "~/Content/mySite.css",
                "~/font-awesome/css/font-awesome.min.css",
                "~/dist/css/skins/_all-skins.min.css",
                "~/dist/css/AdminLTE.min.css",
                "~/Scripts/jstree/themes/default/style.min.css",
                "~/dist/plugins/select2/select2.min.css",
                "~/dist/plugins/select2/select2.bootstrap.css",
                "~/Scripts/jstree/themes/default/style.min.css"
            ));
        }
    }
}
