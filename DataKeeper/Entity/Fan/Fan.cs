using DataKeeper.Config;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace DataKeeper.Entity
{
    //Футбольные клуб
    public partial class Fan : UserControl
    {
        public Fan()
        {
            InitializeComponent();
            initAllFootballer(false, false);

        }
        DataTable dt = new DataTable();
        DataTable dtFan = new DataTable();

        byte select = 0;


        // Выбирает всех футболистов
        void initAllFootballer(bool error = true, bool accept = true)
        {
            if (AppConfig.executeRequestSync(
                @"SELECT * FROM ""Fan""", error))
            {
                //Заполнение таблицы
                dtFan = new DataTable();
                AppConfig.adapter.Fill(dtFan);

                AllFan.DisplayMemberPath = "FullName";
                AllFan.ItemsSource = dtFan.DefaultView;
            }
        }
        // Создает сущность Footballer
        private async void CreateEntity_Click(object sender, RoutedEventArgs e)
        {
            if (await AppConfig.executeRequest(@"CREATE TABLE ""Fan"" (
	            ""Id""	INTEGER NOT NULL,
	            ""IdClub""	INTEGER,
	            ""FullName""	TEXT NOT NULL,
                FOREIGN KEY(""IdClub"") REFERENCES ""FootballClub""(""IdClub""),
	            PRIMARY KEY(""Id"" AUTOINCREMENT));
                INSERT INTO ""Fan""(""IdClub"",""FullName"")
                    VALUES (1,'Валера Никитин Никитович'), (2,'Саша Иванов Иванович');"))
            {
                initAllFootballer(accept: false);

            }

        }

        // Удаляет сущность Footballer
        private async void DeleteEntity_Click(object sender, RoutedEventArgs e)
        {
            await AppConfig.executeRequest(@"DROP TABLE ""Fan""");
            ResultRequest.ItemsSource = null;
            AllFan.ItemsSource = null;
            dtFan = new DataTable();
            dt = new DataTable();
        }

        //Отображает футбольный клуб болельщика
        private async void ViewEntity_Click(object sender, RoutedEventArgs e)
        {
            if (AllFan.SelectedIndex == -1) return;
            if (dtFan.Rows[AllFan.SelectedIndex][1] == DBNull.Value)
            {
                //Заполнение таблицы
                dt = new DataTable();
                ResultRequest.ItemsSource = dt.DefaultView;
            }
            else if (await AppConfig.executeRequest(
                $"SELECT * FROM FootballClub WHERE IdClub={dtFan.Rows[AllFan.SelectedIndex][1]}"))
            {
                //Заполнение таблицы
                dt = new DataTable();
                AppConfig.adapter.Fill(dt);
                ResultRequest.ItemsSource = dt.DefaultView;
            }
        }

        // Добавляет футбольный клуба в список предпочтений болельщика
        private async void AddFootballer_Click(object sender, RoutedEventArgs e)
        {
            if (AllFan.SelectedIndex == -1) return;

            MessageBox.Show("Выберите футбольный клуб, в который хотите добавить болельщика");

            // Выбирает все футбольные клубы
            if (await AppConfig.executeRequest(
                @"SELECT * FROM ""FootballClub"""))
            {
                //Заполнение таблицы
                dt = new DataTable();
                AppConfig.adapter.Fill(dt);
                ResultRequest.ItemsSource = dt.DefaultView;
                select = 1;
                ResultRequest.SelectedCellsChanged += ResultRequest_SelectedCellsChanged;

            }

        }


        //Исключает болельщика из текущего клуба
        private async void ExclusionFootballer_Click(object sender, RoutedEventArgs e)
        {
            if (AllFan.SelectedIndex == -1) return;
            if (dtFan.Rows[AllFan.SelectedIndex][1] == DBNull.Value) return;
            if (MessageBox.Show("Выбранный болельщик будет исключен из текущего клуба",
            "Delete Footballer",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // Изменяет IdClub на NULL у конкретного болельщика
                await AppConfig.executeRequest(
                    $"UPDATE Footballer SET IdClub=NULL WHERE ID={dtFan.Rows[AllFan.SelectedIndex][0]}");
                dtFan.Rows[AllFan.SelectedIndex][1] = DBNull.Value;
                //Заполнение таблицы
                dt = new DataTable();
                ResultRequest.ItemsSource = dt.DefaultView;
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
                             $"UPDATE Fan SET IdClub={dt.Rows[ResultRequest.SelectedIndex][0]}" +
                             $" WHERE ID={dtFan.Rows[AllFan.SelectedIndex][0]}");

                        dtFan.Rows[AllFan.SelectedIndex][1] = dt.Rows[ResultRequest.SelectedIndex][0];

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
