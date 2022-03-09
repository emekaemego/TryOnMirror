using System.Web.Optimization;
using SymaCord.TryOnMirror.UI.Web.Utils;

namespace SymaCord.TryOnMirror.UI.Web.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //    "~/assets/scripts/jquery-1.10.2.js",
            //    "~/assets/scripts/jquery.form.js"
            //                ));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryui")
            //                .Include("~/assets/scripts/jquery-ui-1.10.3.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //    "~/assets/scripts/jquery.validate.js",
            //    "~/assets/scripts/jquery.validate.unobtrusive.js"));

            //bundles.Add(new ScriptBundle(BundleVirtualPath.JqueryFileUploadJs).Include(
            //    "~/assets/scripts/jquery.ui.widget.js",
            //    "~/assets/scripts/jquery.iframe-transport.js",
            //    "~/assets/scripts/jquery.fileupload.js"));

            //bundles.Add(new ScriptBundle("~/bundles/global").Include(
            //    "~/assets/scripts/jquery.easing.1.3.js",
            //    "~/assets/scripts/bootstrap.js",
            //    "~/assets/scripts/jquery.bpopup.js",
            //    "~/assets/scripts/jquery.pnotify.js",
            //    "~/assets/scripts/jquery.linq.js",
            //    "~/assets/scripts/jquery.timeago.js"
            //                //"~/assets/scripts/jquery.qtip.js"
            //                //"~/assets/scripts/imagesloaded.js"
            //                ));

            //bundles.Add(new ScriptBundle("~/bundles/vm-edit").Include(
            //    "~/assets/scripts/jquery.Jcrop.js"
            //                ));

            //bundles.Add(new ScriptBundle("~/bundles/site-js").Include(
            //    "~/assets/scripts/global.js"
            //                ));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/assets/scripts/modernizr-2.5.3.js"));

            //bundles.Add(new StyleBundle("~/assets/css").Include(
            //    //"~/assets/css/jquery.qtip.css",
            //                "~/assets/css/bootmetro-icons.css",
            //                "~/assets/css/bootmetro.css",
            //                "~/assets/css/bootmetro-responsive.css",
            //                "~/assets/css/bootmetro-ui-light.css",
            //                "~/assets/css/jquery.pnotify.default.css",
            //                "~/assets/themes/smoothness/jquery-ui.css",
            //                //"~/assets/css/metro-ui-modern.css",
            //                //"~/assets/css/metro-ui-modern-responsive.css",
            //                "~/assets/css/main.css"
            //                ));

            //bundles.Add(new StyleBundle("~/assets/pager").Include(
            //    "~/assets/css/PagedList.css"
            //                ));


            //New design assets

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/assets/scripts/modernizr")
                            .Include("~/assets/scripts/modernizr-2.5.3.js"));

            bundles.Add(new ScriptBundle("~/assets/scripts/main").Include(
                "~/assets/scripts/jquery-1.10.2.js",
                "~/assets/scripts/bootstrap.js",
                "~/assets/scripts/jquery.pnotify.js"
                            ));

            bundles.Add(new BundleRelaxed(BundleVirtualPath.VirtualMakeoverJs).Include(
                "~/assets/scripts/jquery-ui-1.10.3.js",
                "~/assets/scripts/jquery.ui.widget.js",
                "~/assets/scripts/jquery.iframe-transport.js",
                "~/assets/scripts/jquery.fileupload.js",
                "~/assets/scripts/overlay.js",
                "~/assets/scripts/jquery-ui-timepicker-addon.js",
                "~/assets/scripts/jquery.linq.js",
                "~/assets/scripts/kinetic-v4.6.0.js",
                "~/assets/scripts/virtual.makeover.js",
                "~/assets/scripts/wizard.js",
                "~/assets/scripts/virtual.makeover.operations.js"
                            ));

            bundles.Add(new ScriptBundle("~/assets/scripts/jqueryval").Include(
                "~/assets/scripts/jquery.validate.js",
                "~/assets/scripts/jquery.validate.unobtrusive.js"));

            bundles.Add(new ScriptBundle("~/assets/scripts/site-js").Include(
                "~/assets/scripts/global.js"
                            ));

            bundles.Add(new ScriptBundle("~/assets/scripts/vm-edit").Include(
                "~/assets/scripts/jquery.validate.js",
                "~/assets/scripts/jquery.validate.unobtrusive.js",
                "~/assets/scripts/jquery.form.js",
                "~/assets/scripts/jquery.Jcrop.js",
                "~/assets/scripts/jquery.ui.widget.js",
                "~/assets/scripts/jquery.iframe-transport.js",
                "~/assets/scripts/jquery.fileupload.js"
                            ));

            //Css
            bundles.Add(new StyleBundle("~/assets/css/main").Include(
                "~/assets/css/bootstrap.css",
                "~/assets/css/font-awesome.css",
                "~/assets/css/social-buttons-3.css",
                "~/assets/css/jquery.pnotify.default.css",
                "~/assets/css/yamm.css",
                "~/assets/css/global.css"
                            ));

            bundles.Add(new StyleBundle("~/assets/css/jcrop").Include(
                "~/assets/css/jquery.Jcrop/jquery.Jcrop.css"
                            ));


        bundles.Add(new StyleBundle("~/assets/css/vm-css").Include(
                "~/assets/themes/smoothness/jquery-ui.css",
                //"~/assets/css/jquery.Jcrop/jquery.Jcrop.css",
                "~/assets/css/vm.css",
                "~/assets/css/PagedList.css"
                            ));

            bundles.Add(new StyleBundle("~/assets/css/foundation-icons").Include(
                "~/assets/css/general_foundicons.css"
                //"~/assets/css/social_foundicons.css"
                            ));

            //    bundles.Add(new BundleRelaxed("~/assets/themes/base/css").Include(
            //                "~/Content/themes/base/jquery.ui.core.css",
            //                "~/Content/themes/base/jquery.ui.resizable.css",
            //                "~/Content/themes/base/jquery.ui.selectable.css",
            //                "~/Content/themes/base/jquery.ui.accordion.css",
            //                "~/Content/themes/base/jquery.ui.autocomplete.css",
            //                "~/Content/themes/base/jquery.ui.button.css",
            //                "~/Content/themes/base/jquery.ui.dialog.css",
            //                "~/Content/themes/base/jquery.ui.slider.css",
            //                "~/Content/themes/base/jquery.ui.tabs.css",
            //                "~/Content/themes/base/jquery.ui.datepicker.css",
            //                "~/Content/themes/base/jquery.ui.progressbar.css",
            //                "~/Content/themes/base/jquery.ui.theme.css"));

            //BundleTable.EnableOptimizations = true;
        }

    }
}