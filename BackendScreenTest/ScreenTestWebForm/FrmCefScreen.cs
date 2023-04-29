using CefSharp;
using ScreenTestWebForm.CarModels;
using ScreenTestWebForm.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenTestWebForm
{
    public partial class FrmCefScreen : Form
    {
        Thread th;
        private CefSharp.WinForms.ChromiumWebBrowser browser;
        List<string> listLink = new List<string>();
        JsonOperation jsonOperation = new JsonOperation();
        List<CarModel> listMODELS = new List<CarModel>();
        private HtmlParser htmlParser = new HtmlParser();
        private const string testUrl = "https://www.cars.com/";
        private const string urlLogin = "https://www.cars.com/signin/";
        private bool isPageLoading = false;
        private int pageDetailndex = 0;
        public FrmCefScreen()
        {
            InitializeComponent();
            SetBrowser();

            LoadUrl(urlLogin);
            SetThread();
            StartThread();
        }
        private void SetBrowser()
        {
            browser = new CefSharp.WinForms.ChromiumWebBrowser();
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
            CefSharpSettings.ConcurrentTaskExecution = true;
            CefSharpSettings.FocusedNodeChangedEnabled = true;
            this.browser.LoadingStateChanged += new System.EventHandler<CefSharp.LoadingStateChangedEventArgs>(this.chromiumWebBrowser1_LoadingStateChanged);
            this.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;

        }
        private void LoadUrl(string url)
        {
            browser.Load(url);
        }
        private void SetThread()
        {
            th = new Thread(new ThreadStart(ReadingOperation));
            th.SetApartmentState(ApartmentState.STA);
        }
        private void Wait(int miliSecond)
        {
            DateTime now = DateTime.Now;
            now = now.AddMilliseconds(miliSecond);
            while (now > DateTime.Now && isPageLoading == false)
            {

            }
        }
        private bool isOpetationFinished = false;
        private void StartOperation()
        {
            isOpetationFinished = false;
        }
        private void ReadingOperation()
        {
            StartOperation();
            SetStatusDelegate("Web logining..");
            SetProgressDelegate("0 %");
            LoginWeb();
            SetStatusDelegate("Web reading");
            StartOperation();
            SetStatusDelegate("Setting search parameters");
            SetSelectModel();
            StartOperation();
            SetStatusDelegate("Reading car info from list");
            SetReadCarInfoFromList();
            StartOperation();
            SetStatusDelegate("Reading Page: 2");
            SetClickPage(2);
            StartOperation();
            SetStatusDelegate("Reading Car Details");
            SetReadCarInfoFromList();
            for (int i = 0; i < listLink.Count; i++)
            {
                Double val = Convert.ToDouble(i + 1) / Convert.ToDouble(listLink.Count) * 100;
                SetProgressDelegate(Convert.ToInt32(val) + " %");
                if (pageDetailndex < listLink.Count())
                    SetStatusDelegate("Reading Car Details (link: " + listLink[pageDetailndex] + ",Mdl Count:"+ listMODELS.Count + ")((" + (pageDetailndex + 1).ToString() + "/" + listLink.Count + ") Link Count:" + listLink.Count + ")");
                StartOperation();
                ReadCarInfoDetail();
            }
            SetStatusDelegate("Finished. Saving to json file...");
            SaveToJsonFile();
            SetStatusDelegate("Saved Car info to json file." +  "(Link Count:" + listLink.Count+", Model Count:" +listMODELS.Count+ ")");
        }
        private void LoginWeb()
        {
            Wait(3000);
            while (isOpetationFinished == false)
            {
                LoginUser();
                Thread.Sleep(1000);
            }

        }
        private void SetSelectModel()
        {
            Wait(3000);
            while (isOpetationFinished == false)
            {
                SelectTesla();
                Thread.Sleep(1000);
            }

        }
        private void SetReadCarInfoFromList()
        {
            Wait(3000);
            while (isOpetationFinished == false)
            {
                LoadCarData();
                Thread.Sleep(1000);
            }

        }
        private void SetClickPage(int page)
        {
            Wait(3000);
            while (isOpetationFinished == false)
            {
                ClickPage(page);
                Thread.Sleep(1000);
            }

        }
        private void ReadCarInfoDetail()
        {
            Wait(3000);
            while (isOpetationFinished == false)
            {
                LoadPageDetail();
                Thread.Sleep(1000);
            }

        }
        private void SaveToJsonFile()
        {
            jsonOperation.saveAsJsonFile(listMODELS, "cars-info.json");
        }
        private void setStatus(string text)
        {
            toolStripStatus.Text = text;
        }
        private void setProgress(string text)
        {
            lblProgress.Text = text;
        }
        public void SetStatusDelegate(String text)
        {
            if (InvokeRequired)
            {
                this.Invoke(setStatus, new object[] { text });
                return;
            }
        }
        public void SetProgressDelegate(String text)
        {
            if (InvokeRequired)
            {
                this.Invoke(setProgress, new object[] { text });
                return;
            }
            else
            {
                setProgress(text);
            }
        }

        private async void ClickPage(int pageNumber)
        {
            if (browser.CanExecuteJavascriptInMainFrame)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("let linkBtn=document.getElementById('pagination-direct-link-2');");
                sb.AppendLine("if(linkBtn!=undefined){");
                sb.AppendLine("linkBtn.click();");
                sb.AppendLine("}");
                browser.EvaluateScriptAsync(sb.ToString());
                isOpetationFinished = true;
            }
            else
            {
                isOpetationFinished = false;
            }

        }
        String lastUrl = "";
        private bool againLoading = false;
        private async void LoadPageDetail()
        {
            if (browser.CanExecuteJavascriptInMainFrame)
            {
                if (pageDetailndex < listLink.Count())
                {
                    if (lastUrl != listLink[pageDetailndex] || againLoading)
                    {
                        browser.Load(listLink[pageDetailndex]);
                        againLoading = false;
                    }
                       
                    lastUrl = listLink[pageDetailndex];
                    Wait(8000);
                    Thread.Sleep(1000);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("document.documentElement.innerHTML;");
                    browser.EvaluateScriptAsync(@sb.ToString()).ContinueWith(x =>
                    {
                        var response = x.Result;
                        if (response.Success && response.Result != null)
                        {
                            var html = response.Result;
                            String url = browser.GetBrowser().MainFrame.Url;
                            String[] vals = url.Replace("https://www.cars.com/vehicledetail/", "").Split("/");
                            String id = "";
                            if (vals.Length > 0)
                            {
                                id = vals[0];
                            }
                            //if (browser.GetBrowser().MainFrame.Url.Contains("/vehicledetail") &&
                            //html.ToString().Contains("<dl") &&
                            //html.ToString().Contains("ds-heading--2 heading") &&
                            // html.ToString().Contains("Basics") &&
                            // html.ToString().Contains("Call") &&
                            // html.ToString().Contains("Contact seller"))
                          //  if (browser.GetBrowser().MainFrame.Url.Contains("/vehicledetail") &&
                          //html.ToString().Contains("<dl"))
                          //  {
                                CarModel model = htmlParser.GetCarInfo(html.ToString());
                                if (pageDetailndex >= listLink.Count())
                                {
                                    pageDetailndex = listLink.Count() - 1;
                                }
                                model.Link = listLink[pageDetailndex].Trim();
                                model.Id = id;
                                if (listMODELS.Count() == 0 || listMODELS.FirstOrDefault(y => y.Id == id) == null)
                                {
                                    listMODELS.Add(model);
                                    isPageLoading = false;
                                }
                            if(model.CarInfo!=null)
                                  pageDetailndex++;
                            else
                            {
                                againLoading = true;
                            }
                            //}


                        }

                    });
                    Random random = new Random();

                    Thread.Sleep((random.Next(4)) * 1000 + 7000);
                }
                else
                {



                }
                isOpetationFinished = true;
            }
            else
            {
                isOpetationFinished = false;
            }




        }
        private async void LoginUser()
        {
            if (browser.CanExecuteJavascriptInMainFrame)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("document.getElementById('email').value='johngerson808@gmail.com';");
                sb.AppendLine("document.getElementById('password').value='test8008';");
                sb.AppendLine("let btns=document.getElementsByTagName(\"button\");");
                sb.AppendLine("for(var i=0;i<btns.length;i++){ ");
                sb.AppendLine("  if(btns[i].className=='sds-button'){ ");
                sb.AppendLine("     btns[i].click(); ");
                sb.AppendLine("   }");
                sb.AppendLine("}");

                string scriptSet = sb.ToString();

                browser.EvaluateScriptAsync(scriptSet);
                isOpetationFinished = true;

            }
            else
            {
                isOpetationFinished = false;
            }


            // operationIndex++;


        }
        private async void SelectTesla()
        {
            if (browser.CanExecuteJavascriptInMainFrame)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("document.getElementById('make-model-search-stocktype').value='used';");
                sb.AppendLine("document.getElementById('makes').value='tesla';");
                sb.AppendLine("let buttons=document.getElementsByTagName('button');");
                sb.AppendLine("document.getElementById('make-model-zip').value='94596'");
                sb.AppendLine("document.getElementById('make-model-max-price').value='100000';");
                sb.AppendLine("document.getElementById('make-model-maximum-distance').value='all';");

                sb.AppendLine("document.getElementById('makes').dispatchEvent(new Event('change'));");
                sb.AppendLine("document.getElementById('models').value='tesla-model_s';");
                browser.EvaluateScriptAsync(sb.ToString());
                sb = new StringBuilder();

                sb.AppendLine("for(var i=0;i<buttons.length;i++){ ");
                sb.AppendLine("  if(buttons[i].className=='sds-button'&&buttons[i].getAttribute('data-searchtype')=='make'){ ");
                sb.AppendLine("     buttons[i].click(); ");
                sb.AppendLine("   }");
                sb.AppendLine("}");
                browser.EvaluateScriptAsync(sb.ToString());
                isOpetationFinished = true;
            }
            else
            {
                isOpetationFinished = false;
            }

            //aTimer.Enabled = false;
        }

        private async void LoadCarData()
        {
            if (browser.CanExecuteJavascriptInMainFrame)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("document.documentElement.innerHTML;");
                string dd = browser.GetMainFrame().Url;

                if (dd.Contains("shopping/results/"))
                    browser.EvaluateScriptAsync(@sb.ToString()).ContinueWith(x =>
                    {
                        var response = x.Result;
                        if (response.Success && response.Result != null && response.Result.ToString().Contains("href=\"/vehicledetail"))
                        {
                            var startDate = response.Result;
                            List<String> list = htmlParser.GetAHref(response.Result.ToString());
                            Console.WriteLine(response.Result);
                            if (list.Count > 0)
                            {
                                AddLink(list);

                            }

                        }

                    });
                isOpetationFinished = true;
            }
            else
            {
                isOpetationFinished = false;
            }
        }
        private void AddLink(List<string> lst)
        {
            foreach (var link in lst)
            {
                string[] vals = link.Split('/');
                string id = vals[2];
                if (!listLink.Contains(link) && !listLink.Contains(id))
                {
                    string linkStr = link;

                    if (!linkStr.Contains("https://www.cars.com"))
                    {
                        linkStr = "https://www.cars.com" + link;
                    }
                    if (listLink.Where(x => x.Contains(id)).FirstOrDefault() == null)
                        listLink.Add(linkStr);
                }
            }
        }
        private void StartThread()
        {
            th.Start();
        }
        private void chromiumWebBrowser1_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading && browser.CanExecuteJavascriptInMainFrame)
            {
                isPageLoading = true;

            }

        }
    }
}
