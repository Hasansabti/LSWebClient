using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Webview;
using WpfScreenHelper;

namespace LSWebClient1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
         ConfigUtils config = new ConfigUtils();
        InvoiceData invdata = new InvoiceData();
        DualDisplay ds;
        bool hasdualscreen = false;
   

        public MainWindow()
        {
            Screen s = Screen.AllScreens.First();
            if(Screen.AllScreens.Count() > 1 && config.Read("dualscreen") == "true")
            {
                hasdualscreen = true;
            }
            InitializeComponent();
      
            ds = new DualDisplay();
            if (hasdualscreen)
            {
                ds.initialize(config);
            }

            if(config.Read("fullscreen") == "true")
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
                Application.Current.MainWindow.WindowStyle = WindowStyle.None;
            }


        }

    

        private async void CoreWebView2_WebResourceResponseReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceResponseReceivedEventArgs e)
        {
            try
            {
                if(!hasdualscreen) { return; }

                var respose = await e.Response.GetContentAsync();
                using (var reader = new StreamReader(respose))
                {

                    JObject totalsobject = null;
                    JArray talearray = null;

                    string strContent = reader.ReadToEnd();
                    string jsondecoded = JsonConvert.DeserializeObject(strContent).ToString();
                    JObject resdecoded = (JObject)JsonConvert.DeserializeObject(strContent);
                    JArray ja1 = (JArray)JsonConvert.DeserializeObject(resdecoded["result"].ToString());

                    if (ja1 != null && ja1.Count > 1)
                    {
                        //Search for transaction data
                        foreach (JObject jobject in ja1)
                        {

                            try
                            {
                                JObject result1 = (JObject)jobject;
                                JArray parameters = (JArray)result1.GetValue("parameters");
                                for (int pi = 0; pi<parameters.Count;pi++)
                                {
                                    try
                                    {
                                        JObject methodinvoketotals = null;// (JObject)methodinvoke1[2];
                                        var methodtest = parameters[pi];// (JObject)methodinvoke1[2];

                                        if (parameters[pi] is JArray)
                                        {
                                            JArray methodinvoke1 = (JArray)parameters[pi];
                                            for (int pi2 = 0; pi2 < methodinvoke1.Count; pi2++)
                                            {
                                                try
                                                {
                                                    methodinvoketotals = (JObject)methodinvoke1[pi2];
                                                    if (((JArray)methodinvoketotals.GetValue("Arguments")).ToString().Contains("LSUtils.UpdatePOSTags("))
                                                    {
                                                        JArray iarguments2 = (JArray)methodinvoketotals.GetValue("Arguments");
                                                        JObject ivalue2 = (JObject)iarguments2[0];
                                                        string ijsonval2 = ivalue2.GetValue("Value").ToString();
                                                        JObject iContent2 = (JObject)((JObject)JsonConvert.DeserializeObject(ijsonval2)).GetValue("Content");
                                                        string iJsonContent2 = iContent2.GetValue("Script").ToString();
                                                        if (iJsonContent2.Contains("LSUtils.UpdatePOSTags("))
                                                        {

                                                            //Totals Object
                                                            totalsobject = (JObject)JsonConvert.DeserializeObject(iJsonContent2.Replace("LSUtils.UpdatePOSTags(", "").Replace(");", ""));
                                                            if (totalsobject.ContainsKey("<#Balance>") && totalsobject.GetValue("<#Balance>").ToString() != "")
                                                            {
                                                                invdata.balance = totalsobject.GetValue("<#Balance>").ToString();

                                                            }
                                                            if (totalsobject.ContainsKey("<#Amount>"))
                                                            {
                                                                invdata.amount = totalsobject.GetValue("<#Amount>").ToString();

                                                            }
                                                            if (totalsobject.ContainsKey("<#NetAmount>"))
                                                            {
                                                                invdata.netamount = totalsobject.GetValue("<#NetAmount>").ToString();

                                                            }
                                                            if (totalsobject.ContainsKey("<#TaxAmount>"))
                                                            {
                                                                invdata.vat = totalsobject.GetValue("<#TaxAmount>").ToString();

                                                            }
                                                            if (totalsobject.ContainsKey("<#Discount>"))
                                                            {
                                                                invdata.discount = totalsobject.GetValue("<#Discount>").ToString();

                                                            }
                                                            if (totalsobject.ContainsKey("<#State>"))
                                                            {
                                                                invdata.state = totalsobject.GetValue("<#State>").ToString();

                                                            }
                                                            if (totalsobject.ContainsKey("<#State>"))
                                                            {
                                                                invdata.state = totalsobject.GetValue("<#State>").ToString();
                                                                if (totalsobject.GetValue("<#State>").ToString() == "")
                                                                {
                                                                    //POS Closed

                                                                    ds.logout();
                                                                }
                                                            }


                                                            ds.updateData(invdata);
                                                        }

                                                    }

                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine(ex.ToString());
                                                }
                                            }
                                        }
                                        else
                                        {
                                            methodinvoketotals = (JObject)parameters[pi];

                                            JArray arguments2 = (JArray)methodinvoketotals.GetValue("Arguments");
                                            JObject value2 = (JObject)arguments2[0];
                                            string jsonval2 = value2.GetValue("Value").ToString();
                                            JObject Content2 = (JObject)((JObject)JsonConvert.DeserializeObject(jsonval2)).GetValue("Content");
                                            string JsonContent2 = Content2.GetValue("Script").ToString();
                                            if (JsonContent2.Contains("LSUtils.UpdatePOSTags("))
                                            {

                                                //Totals Object
                                                totalsobject = (JObject)JsonConvert.DeserializeObject(JsonContent2.Replace("LSUtils.UpdatePOSTags(", "").Replace(");", ""));
                                                if (totalsobject.ContainsKey("<#Balance>") && totalsobject.GetValue("<#Balance>").ToString() != "")
                                                {
                                                    invdata.balance = totalsobject.GetValue("<#Balance>").ToString();

                                                }
                                                if (totalsobject.ContainsKey("<#Amount>"))
                                                {
                                                    invdata.amount = totalsobject.GetValue("<#Amount>").ToString();

                                                }
                                                if (totalsobject.ContainsKey("<#NetAmount>"))
                                                {
                                                    invdata.netamount = totalsobject.GetValue("<#NetAmount>").ToString();

                                                }
                                                if (totalsobject.ContainsKey("<#TaxAmount>"))
                                                {
                                                    invdata.vat = totalsobject.GetValue("<#TaxAmount>").ToString();

                                                }
                                                if (totalsobject.ContainsKey("<#Discount>"))
                                                {
                                                    invdata.discount = totalsobject.GetValue("<#Discount>").ToString();

                                                }
                                                if (totalsobject.ContainsKey("<#State>"))
                                                {
                                                    invdata.state = totalsobject.GetValue("<#State>").ToString();

                                                }
                                                if (totalsobject.ContainsKey("<#State>"))
                                                {
                                                    invdata.state = totalsobject.GetValue("<#State>").ToString();
                                                    if (totalsobject.GetValue("<#State>").ToString() == "")
                                                    {
                                                        //POS Closed

                                                        ds.logout();
                                                    }
                                                }


                                                ds.updateData(invdata);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.ToString());
                                    }
                                    try { 

                                    JObject methodinvoke2 = null;// (JObject)methodinvoke1[1];

                                        if (parameters[pi] is JArray)
                                        {
                                            JArray methodinvoke1 = (JArray)parameters[pi];
                                            for (int pi2 = 0; pi2 < methodinvoke1.Count; pi2++)
                                            {
                                                try
                                                {
                                                    methodinvoke2 = (JObject)methodinvoke1[pi2];
                                                    if (((JArray)methodinvoke2.GetValue("Arguments")).ToString().Contains("imageColumns"))
                                                    {
                                                        break;
                                                    }

                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine(ex.ToString());
                                                }
                                            }
                                        }
                                        else
                                        {
                                            methodinvoke2 = (JObject)parameters[pi];
                                        }


                       
                                        JArray arguments1 = (JArray)methodinvoke2.GetValue("Arguments");
                                    JObject value = (JObject)arguments1[0];
                                    string jsonval = value.GetValue("Value").ToString();
                                    JObject Content = (JObject)((JObject)JsonConvert.DeserializeObject(jsonval)).GetValue("Content");
                                    JObject JsonContent = ((JObject)JsonConvert.DeserializeObject(Content.GetValue("Json").ToString()));


                                    //Item Array
                                    talearray = (JArray)JsonContent.GetValue("data");
                                    DataTable itemdt = new DataTable();
                                    itemdt.Columns.Add("num");
                                    itemdt.Columns.Add("barcode");
                                    itemdt.Columns.Add("desc2");
                                    itemdt.Columns.Add("unit");
                                    itemdt.Columns.Add("qty");
                                    itemdt.Columns.Add("price");
                                    itemdt.Columns.Add("amount");
                                    itemdt.Columns.Add("data1");
                                    itemdt.Columns.Add("data2");

                                    for (int i = 0; i < talearray.Count; i++)
                                    {
                                        List<string> strings = new List<string>();
                                        foreach (string val in talearray[i])
                                        {
                                            // if(val.Contains)
                                            strings.Add(val);
                                        }
                                        itemdt.Rows.Add(strings.ToArray());
                                    }

                                    invdata.lines = itemdt;

                                    ds.updateData(invdata);









                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.ToString());
                                    }
                                }
                            }
                            catch (Exception ex) {
                                Console.WriteLine(ex.ToString());
                            }
                        }
                    }


                    //strContent.du

                }
              
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            }

        private async void wv2_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            wv2.CoreWebView2.BasicAuthenticationRequested += CoreWebView2_BasicAuthenticationRequested;
            wv2.CoreWebView2.WebResourceResponseReceived += CoreWebView2_WebResourceResponseReceived;
            wv2.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
      

         
          


        }

        private async void CoreWebView2_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
      
        }

        private void CoreWebView2_BasicAuthenticationRequested(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2BasicAuthenticationRequestedEventArgs e)
        {
            e.Response.UserName = config.Read("username");
            e.Response.Password = config.Read("password");
        }

        private async void wv2_MouseDown(object sender, MouseButtonEventArgs e)
        {
     
        }

        private async void wv2_ContentLoading(object sender, Microsoft.Web.WebView2.Core.CoreWebView2ContentLoadingEventArgs e)
        {
     
        }

        private async void wv2_Initialized(object sender, EventArgs e)
        {
            await wv2.EnsureCoreWebView2Async(null);
            string link = config.Read("serverlink");
            wv2.CoreWebView2.Navigate(link);
        }
    }
}
