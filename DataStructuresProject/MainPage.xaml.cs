using Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DataStructuresProject
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Manager _manager;       
        public MainPage()
        {
            this.InitializeComponent();
            var config = new Configuration();
            _manager = new Manager(config.Data.MaxBoxes, config.Data.LowAmountInBox, new TimeSpan(00, 0, 5), new TimeSpan(00, 0, 60));            
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            double x, y;
            int amount;
            bool wasSuccessful;
            int wereAdded;

            if (!double.TryParse(X_Txt.Text, out x) || x <= 0)
            {
                await new MessageDialog($"You should enter a positive number in the Width and Length textbox").ShowAsync();
                X_Txt.Text = "";
                return;
            }
            if (!double.TryParse(Y_Txt.Text, out y) || y <= 0)
            {
                await new MessageDialog($"You should enter a positive number in the Height textbox").ShowAsync();
                Y_Txt.Text = "";
                return;
            }
            if (!int.TryParse(Amount_Txt.Text, out amount) || amount <= 0)
            {
                await new MessageDialog($"You should enter a positive number in the Amount textbox").ShowAsync();
                Amount_Txt.Text = "";
                return;
            }
            _manager.Add(x, y, amount, out wasSuccessful, out wereAdded);
            X_Txt.Text = "";
            Y_Txt.Text = "";
            Amount_Txt.Text = "";
            if (wasSuccessful == true)
                await new MessageDialog($"{wereAdded} units of a box with Width & Length of {x} and Height of {y} were added successfully").ShowAsync();
            if (wasSuccessful == false)
                await new MessageDialog($"The amount of boxes you wanted to add was too high, our max capacity per box is {_manager.MaxItemsPerBox},\n" +
                    $"therefore only {wereAdded} boxes were added and the rest were returned to the supplier").ShowAsync();
        }
        private async void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            double x, y;
            const int YES = 1, NO = 2;
            int amount;           
            bool matchData, boxWasDeleted, someBoxWasDeleted;
            List<Box> boxes = new List<Box>();
            StringBuilder sb = new StringBuilder();
            StringBuilder sbDeleted = new StringBuilder();
            if (!double.TryParse(XBuy_Txt.Text, out x) || x <= 0)
            {
                await new MessageDialog($"You should enter a positive number in the Width and Length textbox").ShowAsync();
                XBuy_Txt.Text = "";
                return;
            }
            if (!double.TryParse(YBuy_Txt.Text, out y) || y <= 0)
            {
                await new MessageDialog($"You should enter a positive number in the Height textbox").ShowAsync();
                YBuy_Txt.Text = "";
                return;
            }
            if (!int.TryParse(AmountBuy_Txt.Text, out amount) || amount <= 0)
            {
                await new MessageDialog($"You should enter a positive number in the Amount textbox").ShowAsync();
                AmountBuy_Txt.Text = "";
                return;
            }
            _manager.Buy(x, y, amount, out matchData, out sb, out boxes);
            XBuy_Txt.Text = "";
            YBuy_Txt.Text = "";
            AmountBuy_Txt.Text = "";

            if (matchData == false)
            {
                await new MessageDialog($"There are no boxes with available units up to 4 in Width/Length and up to 4 in Height to fulfill your request").ShowAsync();
                return;
            }

            var dialog = new MessageDialog($"{sb}");
            dialog.Commands.Add(new UICommand { Label = "Yes", Id = YES });
            dialog.Commands.Add(new UICommand { Label = "No", Id = NO });
            var result = await dialog.ShowAsync();
            var id = (int)result.Id;
            if (id == NO)
                return;
            if (id == YES)
            {
                _manager.ChangeBoughtBoxes(boxes, out boxWasDeleted, out sbDeleted, out someBoxWasDeleted);
                if(someBoxWasDeleted == true)
                {
                    await new MessageDialog($"{sbDeleted}").ShowAsync();
                    return;
                }
                if (boxWasDeleted == false)
                {
                    await new MessageDialog($"Sorry, the box you wished to buy of: Length / Width of {x} & Height of: {y} was deleted " +
                        $"because it wasn't bought for more than {_manager.ExpirationDate}").ShowAsync();
                    return;
                }
            }
        }
        private async void BtnGetInfo_Click(object sender, RoutedEventArgs e)
        {
            double x, y;
            int amount;
            bool boxExist, lowAmount;
            if (!double.TryParse(XInfo_Txt.Text, out x) || x <= 0)
            {
                await new MessageDialog($"You should enter a positive number in the Width and Length textbox").ShowAsync();
                XInfo_Txt.Text = "";
                return;
            }
            if (!double.TryParse(YInfo_Txt.Text, out y) || y <= 0)
            {
                await new MessageDialog($"You should enter a positive number in the Height textbox").ShowAsync();
                YInfo_Txt.Text = "";
                return;
            }
            _manager.Info(x, y, out boxExist, out lowAmount, out amount);
            XInfo_Txt.Text = "";
            YInfo_Txt.Text = "";

            if (boxExist == true && lowAmount == true)
                await new MessageDialog($"The box with Width & Length of {x} and Height of {y} has only {amount} units in stock, restock ASAP").ShowAsync();
            else if (boxExist == true && lowAmount == false)
                await new MessageDialog($"The box with Width & Length of {x} and Height of {y} has {amount} units in stock").ShowAsync();
            else
                await new MessageDialog($"The box (Width & Length of {x} and Height of {y}) doesn't exist").ShowAsync();
        }
        private async void BtnShowOldBoxes_Click(object sender, RoutedEventArgs e)
        {
            int hours, minutes, seconds;

            if (!int.TryParse(Hours_Txt.Text, out hours) || hours < 0 || hours > 24)
            {
                await new MessageDialog($"You should enter a number between 0 to 24 in the Hours textbox").ShowAsync();
                Hours_Txt.Text = "";
                return;
            }
            if (!int.TryParse(Minutes_Txt.Text, out minutes) || minutes < 0 || minutes > 59)
            {
                await new MessageDialog($"You should enter a number between 0 to 59 in the Minutes textbox").ShowAsync();
                Minutes_Txt.Text = "";
                return;
            }
            if (!int.TryParse(Seconds_Txt.Text, out seconds) || seconds < 0 || seconds > 59)
            {
                await new MessageDialog($"You should enter a number between 0 to 59 in the Seconds textbox").ShowAsync();
                Seconds_Txt.Text = "";
                return;
            }
            if (seconds == 0 && minutes == 0 && hours == 0)
            {
                await new MessageDialog($"You entered invalid time").ShowAsync();
                return;
            }

            await new MessageDialog($"{_manager.ShowOldBoxes(hours, minutes, seconds)}").ShowAsync();
            Hours_Txt.Text = "";
            Minutes_Txt.Text = "";
            Seconds_Txt.Text = "";
        }
    }
}
