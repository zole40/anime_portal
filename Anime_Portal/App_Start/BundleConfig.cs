using System.Web;
using System.Web.Optimization;

namespace Anime_Portal
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/angular.min.js")
                .Include("~/Scripts/angular-route.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval")
                .Include("~/Scripts/jquery.validate*")
                .Include("~/Scripts/AnimePortal.Validate.js"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                .Include("~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js")
                .Include("~/Scripts/jasny-bootstrap.*")
                .Include("~/Scripts/datatables/jquery.dataTables.js")
                .Include("~/Scripts/datatables/dataTables.bootstrap.js")
                .Include("~/Scripts/AdminLTE/app.js"));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/bootstrap.min.css")
                .Include("~/Content/Site.css")
                .Include("~/Content/font-awesome.min.css")
                .Include("~/Content/ionicons.min.css")
                .Include("~/Content/AdminLTE.css")
                .Include("~/Content/datatables/dataTables.bootstrap.css"));
        }
    }
}
