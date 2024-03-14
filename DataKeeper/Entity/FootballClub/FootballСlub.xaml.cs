using DataKeeper.Config;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace DataKeeper.Entity
{
    //Футбольные клуб
    public partial class FootballClub : UserControl
    {
        public FootballClub()
        {
            InitializeComponent();
            initAllFootballClub(false, false);


        }
        DataTable dt = new DataTable();
        static DataTable dtFootballClub = new DataTable();

        byte select = 0;

        // Выбирает все футбольные клубы
        void initAllFootballClub(bool error = true, bool accept = true)
        {
            if (AppConfig.executeRequestSync(
                @"SELECT * FROM ""FootballClub""", error))
            {
                //Заполнение таблицы
                dtFootballClub = new DataTable();
                AppConfig.adapter.Fill(dtFootballClub);
                
                AllFootballClub.DisplayMemberPath = "Name";
                AllFootballClub.ItemsSource = dtFootballClub.DefaultView;
            }
        }


        // Создает сущность FootballClub
        private async void CreateEntity_Click(object sender, RoutedEventArgs e)
        {
            if (await AppConfig.executeRequest(@"CREATE TABLE ""FootballClub"" (
	            ""IdClub""	INTEGER NOT NULL,
	            ""Name""	TEXT NOT NULL,
	            ""City""	TEXT NOT NULL,
	            PRIMARY KEY(""IdClub"" AUTOINCREMENT));
                INSERT INTO ""FootballClub""(""IdClub"",""Name"",""City"") 
                    VALUES (1,'Имя клуба','Имя города'),(2,'Имя клуба_2','Имя города_2');"))
            {
                initAllFootballClub(accept: false);
            }
        }

        // Удаляет сущность FootballClub
        private async void DeleteEntity_Click(object sender, RoutedEventArgs e)
        {
            await AppConfig.executeRequest(@"DROP TABLE ""FootballClub""");
            ResultRequest.ItemsSource = null;
            AllFootballClub.ItemsSource = null;
            dtFootballClub = new DataTable();
            dt = new DataTable();
        }

        //Отображает всех футболистов из выбранного клуба
        private async void ViewEntity_Click(object sender, RoutedEventArgs e)
        {
            // Выбирает всех футболистов из выбранного футбольного клуба
            if (AllFootballClub.SelectedIndex == -1) return;
            if (await AppConfig.executeRequest(
                $"SELECT * FROM Footballer WHERE IdClub={dtFootballClub.Rows[AllFootballClub.SelectedIndex][0]}"))
            {
                //Заполнение таблицы
                dt = new DataTable();
                AppConfig.adapter.Fill(dt);
                ResultRequest.ItemsSource = dt.DefaultView;
            }

        }

        //Исключить футболиста из ранее выбранного клуба
        private async void ExclusionFootballer_Click(object sender, RoutedEventArgs e)
        {
            if (AllFootballClub.SelectedIndex == -1) return;
            if (MessageBox.Show("Выбранный футболист будет исключен из выбранного клуба",
            "Delete Footballer",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // Изменяет IdClub на NULL у конкретного футболиста
                if (ResultRequest.SelectedIndex == -1) return;

                if (await AppConfig.executeRequest(
                    $"UPDATE Footballer SET IdClub=NULL WHERE IdClub={dtFootballClub.Rows[AllFootballClub.SelectedIndex][0]}"))
                {
                    dt.Rows.RemoveAt(ResultRequest.SelectedIndex);
                }


            }
        }

        // Добавляет футболиста в ранее выбранный клуб
        private async void AddFootballer_Click(object sender, RoutedEventArgs e)
        {
            if (AllFootballClub.SelectedIndex == -1) return;

            MessageBox.Show("Выберите футболиста, которого хотите добавит в выбранный клуб");

            // Выбирает всех футболистов
            if (await AppConfig.executeRequest(
                @"SELECT * FROM ""Footballer"""))
            {
                //Заполнение таблицы
                dt = new DataTable();
                AppConfig.adapter.Fill(dt);
                ResultRequest.ItemsSource = dt.DefaultView;
                select = 1;
                ResultRequest.SelectedCellsChanged += ResultRequest_SelectedCellsChanged;

            }

        }
        private async void ResultRequest_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            ResultRequest.SelectedCellsChanged -= ResultRequest_SelectedCellsChanged;

            switch (select)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {

                        // Изменяет IdClub на выбранный клуб у конкретного футболиста
                        await AppConfig.executeRequest(
                             $"UPDATE Footballer SET IdClub={dtFootballClub.Rows[AllFootballClub.SelectedIndex][0]}" +
                             $" WHERE ID={dt.Rows[ResultRequest.SelectedIndex][0]}");

                        //Заполнение таблицы
                        dt = new DataTable();
                        ResultRequest.ItemsSource = dt.DefaultView;
                        break;
                    }

            }
            select = 0;


        }


    }
}
