using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoClicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, KeyBindingCallback
    {
        private HookManager hookManager;
        private ClickManager clickManager;
        private AppSettings appSettings;
        private string fileName = "settings.json";
        public MainWindow()
        {
            InitializeComponent();
            TryToLoadSettings();
            butKeyBinded.Content = appSettings.bindedKey;
            clickManager = new ClickManager();
            hookManager = new HookManager();
            hookManager.setOnKeyBindingChangeListener(this);
        }


        private void TryToLoadSettings()
        {
            
            if (File.Exists(fileName))
            {
                string jsonString = File.ReadAllText(fileName);
                appSettings = JsonSerializer.Deserialize<AppSettings>(jsonString);
                textFocusedWindowName.Text = appSettings.FocusedWindowName;
                textMaxClickSpeed.Text = appSettings.Max.ToString();
                textMinClickSpeed.Text = appSettings.Min.ToString();
                checkIsSimulateRealClick.IsChecked = appSettings.IsSimulateHumanClick;
                butKeyBinded.Content = appSettings.bindedKey;
            }
            else
            {
                appSettings = new AppSettings()
                {
                    bindedKey = Keys.Home
                };
            }
        }

        private void SaveSettings()
        {
            appSettings.FocusedWindowName = textFocusedWindowName.Text;
            appSettings.IsSimulateHumanClick = checkIsSimulateRealClick.IsChecked.Value;
            appSettings.Min = int.Parse(textMinClickSpeed.Text);
            appSettings.Max = int.Parse(textMaxClickSpeed.Text);
            appSettings.bindedKey = appSettings.bindedKey;
            string jsonString = JsonSerializer.Serialize(appSettings);
            File.WriteAllText(fileName, jsonString);
        }

        private void BeginKeyBinding(object sender, RoutedEventArgs e)
        {
            if (!clickManager.IsAutoClickEnabled)
            {
                HookManager.isChangingKey = true;
                butKeyBinded.Content = "";
            }
        }

        public Keys getSetKeyBinding()
        {
            return appSettings.bindedKey;
        }

        public void onBindedKeyPush()
        {
            if (clickManager.IsAutoClickEnabled)
            {
                clickManager.StopAutoClicker();
            }
            else
            { 
                SaveSettings();
                clickManager.StartAutoClicker(appSettings.FocusedWindowName, appSettings.Min, appSettings.Max, appSettings.IsSimulateHumanClick);
            }
        }

        public void onKeyBindingChange(Keys binding)
        {
            HookManager.isChangingKey = false;
            appSettings.bindedKey = binding;
            butKeyBinded.Content = binding;
        }

        private void SaveSettingsClick(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }
    }
}
