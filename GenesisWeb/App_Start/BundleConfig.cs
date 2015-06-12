using System.Web;
using System.Web.Optimization;

namespace GenesisWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(                     
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-1.11.4/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //Bootstrap.js was interferring with jQuery (especially in tooltips)
            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                    "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/spcontext").Include(
                        "~/Scripts/spcontext.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Scripts/jquery-ui-1.11.4/jquery-ui.css",
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/chromeloader").Include(
                    "~/Scripts/chromeloader.js"));


             bundles.Add(new ScriptBundle("~/bundles/app").Include(
               "~/App/app.js",
               "~/App/templates.js",
              // "~/App/models/*.js",
               "~/App/routes/*.js",
               "~/App/controllers/*.js",
               "~/App/views/*.js"));

             bundles.Add(new ScriptBundle("~/bundles/ember").Include(
              "~/Scripts/handlebars.js",
              "~/Scripts/ember.js"));            

          
        }       

    }
}
