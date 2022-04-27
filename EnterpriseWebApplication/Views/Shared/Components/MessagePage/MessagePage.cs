using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace XTLASPNET
{
    [ViewComponent]
    public class MessagePage : ViewComponent
    {
        public const string COMPONENTNAME = "MessagePage";
        public class Message {
            public string title {set; get;} = "Alert";
            public string htmlcontent {set; get;} = "";
            public string urlredirect {set; get;} = "/";
            public int secondwait {set; get;} = 2;
        }
        public MessagePage() {}
        public IViewComponentResult Invoke(Message message) {
            this.HttpContext.Response.Headers.Add("REFRESH",$"{message.secondwait};URL={message.urlredirect}");
            return  View(message);
        }
    }
}
