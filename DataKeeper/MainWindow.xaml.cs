using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Data.Common;
using System.Windows.Data;
using System.Reflection.PortableExecutable;
using System.IO;
using DataKeeper.Config;
using DataKeeper.Entity;
using System.Diagnostics;

namespace DataKeeper
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Entity.ItemsSource = new List<string>()
            {
                "Футбольные клуб",
                "Футболисты",
                "Болельщики"
            };
            while (true)
            {
                try
                {
                    if (File.Exists(AppConfig.path))
                    {
                        if (MessageBox.Show("Удалить файл базы данных?",
                            "Delete file",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            File.Delete(AppConfig.path);

                        }


                    }
                    AppConfig.con.Open();
                    break;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка:\n{ex.Message}");
                }
            }

        }




        private void Entity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                switch (Entity.SelectedIndex)
                {
                    case 0:
                        {
                            MyContentControl.Content = new FootballClub();
                            break;
                        }
                    case 1:
                        {
                            MyContentControl.Content = new Footballer();
                            break;
                        }
                    case 2:
                        {
                            MyContentControl.Content = new Fan();
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка:\n{ex.Message}");
            }
        }
    }
}
