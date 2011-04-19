using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace Rabbit
{
    public class Controller
    {
        internal dynamic Model { get; set; }

        internal object RenderView(string view, object model)
        {
            var viewHook = string.Format("GET_{0}_{1}_{2}_View", ModuleName, ContentType, view);
            var pageHook = string.Format("{0}_{1}_{2}", ModuleName, ContentType, view);
            var defaultView = string.Format("~/Views/{0}/_{1}_{2}.cshtml", ModuleName, ContentType, view);
            dynamic ret = new ExpandoObject();
            ret.ViewHook = viewHook;
            ret.PageHook = pageHook;
            ret.Model = model;
            ret.DefaultView = defaultView;
            return ret;
        }

        /// <summary>
        /// To call this function, parameter cannot be dynamic
        /// It has to be casted to ExpandoObject if it is dynamic. 
        /// e.g. it should be used like this: RenderView((ExpandoObject) data)
        /// Otherwise the view name will be CallSite.Target
        /// </summary>
        /// <param name="model"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        protected object RenderView(ExpandoObject model)
        {
            var st = new StackTrace(1);
            var view = st.GetFrame(0).GetMethod().Name;
            return RenderView(view, model);
        }

        protected object Redirect(string path)
        {
            dynamic ret = new ExpandoObject();
            ret.Redirect = string.Format("~/{0}/{1}", Name, path);
            return ret;
        }

        protected string Name { get; set; }
        protected string ModuleName { get; set; }
        protected string ContentType { get; set; }

        public string GET_LIST { get { return string.Format("GET_{0}_{1}_List", ModuleName, ContentType); } }
        public string GET_ITEM { get { return string.Format("GET_{0}_{1}", ModuleName, ContentType); } }
        public string DELETE_ITEM { get { return string.Format("DELETE_{0}_{1}", ModuleName, ContentType); } }
        public string UPDATE_ITEM { get { return string.Format("UPDATE_{0}_{1}", ModuleName, ContentType); } }
        public string NEW_ITEM { get { return string.Format("NEW_{0}_{1}", ModuleName, ContentType); } }
        public string CREATE_ITEM { get { return string.Format("CREATE_{0}_{1}", ModuleName, ContentType); } }
        public string VALIDATE_ITEM { get { return string.Format("VALIDATE_{0}_{1}", ModuleName, ContentType); } }
    }
}