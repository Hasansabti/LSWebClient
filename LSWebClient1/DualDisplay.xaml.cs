using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfScreenHelper;

namespace LSWebClient1
{
    /// <summary>
    /// Interaction logic for DualDisplay.xaml
    /// </summary>
    public partial class DualDisplay : Window
    {
        InvoiceData invdata = new InvoiceData();
        public DualDisplay()
        {
           

            InitializeComponent();
       

        }

        public void initialize(ConfigUtils config)
        {
            if (Screen.AllScreens.Count() > 1)
            {
                Screen s = Screen.AllScreens.ElementAt(int.Parse(config.Read("dualscreennum")));
                System.Windows.Rect r = s.WorkingArea;
                Top = r.Top;
                Left = r.Left+10;
                Show();
                this.WindowState = System.Windows.WindowState.Maximized;


                using (var webClient = new System.Net.WebClient())
                {
                    // webClient.Headers.Add("Authorization", "Bearar " + PmConstants.apikey); 
                 //   webClient.Encoding = Encoding.GetEncoding("UTF-8");
                 //   string url = "https://ads.alamer-market.com/api/listingAds/?category=SP-01";
                  //  var json = webClient.DownloadString(url);
                  //  Console.WriteLine(json.ToString());
                  // JsonObject json = JsonConvert.DeserializeObject(json);
                    
                    // Now parse with JSON.Net
                    // DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
                    // return dt;
                }

            }
        }

        internal void logout()
        {
          this.invdata.lines.Clear();
  
            gc1.ItemsSource = this.invdata.lines;
            Overlay.Visibility = Visibility.Visible;
        }

        internal void updateData(InvoiceData invdata)
        {
            if (invdata.state != null && invdata.state != "")
            {
                this.invdata = invdata;
                gc1.ItemsSource = this.invdata.lines;
                Overlay.Visibility = Visibility.Collapsed;
                balance.Text = invdata.balance;
                vat.Text = invdata.vat;
                total.Text = invdata.amount;
            }
            else
            {
                logout();
            }

        }

        private void tv1_CanSelectRow(object sender, DevExpress.Xpf.Grid.CanSelectRowEventArgs e)
        {
            e.CanSelectRow = false;
        }
    }
}
