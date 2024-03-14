using DataKeeper.Config;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace DataKeeper.Entity
{
    //Футбольные клуб
    public partial class Footballer : UserControl
    {
        public Footballer()
        {
            InitializeComponent();
            initAllFootballer(false, false);

        }
        DataTable dt = new DataTable();
        DataTable dtFootballer = new DataTable();

        byte select = 0;


        // Выбирает всех футболистов
        void initAllFootballer(bool error = true, bool accept = true)
        {
            if (AppConfig.executeRequestSync(
                @"SELECT * FROM ""Footballer""", error))
            {
                //Заполнение таблицы
                dtFootballer = new DataTable();
                AppConfig.adapter.Fill(dtFootballer);

                AllFootballer.DisplayMemberPath = "FullName";
                AllFootballer.ItemsSource = dtFootballer.DefaultView;
            }
        }
        // Создает сущность Footballer
        private async void CreateEntity_Click(object sender, RoutedEventArgs e)
        {
            if (await AppConfig.executeRequest(
                @"CREATE TABLE ""Footballer"" (
                ""Id""	INTEGER NOT NULL,
                ""IdClub""	INTEGER,
                ""FullName""	TEXT NOT NULL,
                ""BirthDate""	TEXT NOT NULL,
                ""SNILSNumber""	TEXT NOT NULL,
                FOREIGN KEY(""IdClub"") REFERENCES ""FootballClub""(""IdClub""),
                PRIMARY KEY(""Id"" AUTOINCREMENT);
                INSERT INTO ""Footballer""(""IdClub"",""FullName"",""BirthDate"",""SNILSNumber"")
                    VALUES (1,'Никита Никитин Никитович','09.02.1996','2313213'), (2,'Иван Иванов Иванович','10.10.2000','2313213');"))
            {
                initAllFootballer(accept: false);

            }

        }

        // Удаляет сущность Footballer
        private async void DeleteEntity_Click(object sender, RoutedEventArgs e)
        {
            await AppConfig.executeRequest(@"DROP TABLE ""Footballer""");
            ResultRequest.ItemsSource = null;
            ResultRequest.ItemsSource = null;
            AllFootballer.ItemsSource = null;
            dtFootballer = new DataTable();
            dt = new DataTable();
        }


        //Отображает футбольный клуб к которому принадлежит футболист
        private async void ViewEntity_Click(object sender, RoutedEventArgs e)
        {
            if (AllFootballer.SelectedIndex == -1) return;
            if (dtFootballer.Rows[AllFootballer.SelectedIndex][1] == DBNull.Value || await AppConfig.executeRequest(
                $"SELECT * FROM FootballClub WHERE IdClub={dtFootballer.Rows[AllFootballer.SelectedIndex][1]}"))
            {
                //Заполнение таблицы
                dt = new DataTable();
                AppConfig.adapter.Fill(dt);
                ResultRequest.ItemsSource = dt.DefaultView;
            }


        }
        // Переводит футболиста в выбранный клуб
        private async void AddFootballer_Click(object sender, RoutedEventArgs e)
        {
            if (AllFootballer.SelectedIndex == -1) return;

            MessageBox.Show("Выберите футбольный клуб, в который хотите добавить футболиста");

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


        //Исключить футболиста из текущего клуба
        private async void ExclusionFootballer_Click(object sender, RoutedEventArgs e)
        {
            if (AllFootballer.SelectedIndex == -1) return;
            if (dtFootballer.Rows[AllFootballer.SelectedIndex][1] == DBNull.Value) return;
            if (MessageBox.Show("Выбранный футболист будет исключен из текущего клуба",
            "Delete Footballer",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // Изменяет IdClub на NULL у конкретного футболиста
                await AppConfig.executeRequest(
                    $"UPDATE Footballer SET IdClub=NULL WHERE ID={dtFootballer.Rows[AllFootballer.SelectedIndex][0]}");
                dtFootballer.Rows[AllFootballer.SelectedIndex][1] = DBNull.Value;
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
                             $"UPDATE Footballer SET IdClub={dt.Rows[ResultRequest.SelectedIndex][0]}" +
                             $" WHERE ID={dtFootballer.Rows[AllFootballer.SelectedIndex][0]}");

                        dtFootballer.Rows[AllFootballer.SelectedIndex][1] = dt.Rows[ResultRequest.SelectedIndex][0];

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
