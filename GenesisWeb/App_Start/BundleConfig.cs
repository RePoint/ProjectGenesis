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
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/spcontext").Include(
                        "~/Scripts/spcontext.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/chromeloader").Include(
                    "~/Scripts/chromeloader.js"));
          

            if (!HttpContext.Current.IsDebuggingEnabled)
            {
                bundles.Add(new Bundle("~/bundles/templates",
                    new EmberHandlebarsBundleTransform())
                    .IncludeDirectory("~/App/templates\\", "*.hbs", true)
                );
            }

             bundles.Add(new ScriptBundle("~/bundles/app").Include(
               "~/App/app.js",
               "~/App/models/*.js",
               "~/App/routes/*.js",
               "~/App/controllers/*.js",
               "~/App/views/*.js"));

             bundles.Add(new ScriptBundle("~/bundles/ember").Include(
              "~/Scripts/handlebars.js",
              "~/Scripts/ember-template-compiler.js",
              "~/Scripts/ember.js"));
            

          
        }       

    }
}
