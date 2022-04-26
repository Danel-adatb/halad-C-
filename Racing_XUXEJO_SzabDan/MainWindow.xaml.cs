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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;

namespace Racing_XUXEJO_SzabDan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Változók amiket fel fogunk használni
        DispatcherTimer gameTimer = new DispatcherTimer();
        List<Rectangle> itemRemover = new List<Rectangle>();

        Random rand = new Random();

        ImageBrush playerImage = new ImageBrush();
        ImageBrush starImage = new ImageBrush();

        Rect playerHitBox;

        int speed = 15;
        int playerSpeed = 10;
        int carNum;
        int startCounter = 30;
        int powerModeCounter = 200;

        double score;
        double i;

        bool moveLeft, moveRight, gameOver, powerMode;

        public MainWindow()
        {
            InitializeComponent();
            myCanvas.Focus();

            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);

            StartGame();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            score += .05;

            startCounter -= 1;

            scoreTxt.Content = "Survived: " + score.ToString("#.#") + " Seconds";

            //Hitbox mely segítségével érzékeltethük, ha valamivel volt kollózió
            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);

            //Az autónk nem mehet ki a mainWindow-ból
            if (moveLeft == true && Canvas.GetLeft(player) > 0) Canvas.SetLeft(player, Canvas.GetLeft(player) - playerSpeed);
            if (moveRight == true && Canvas.GetLeft(player) + 90 < Application.Current.MainWindow.Width) Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);
        
            //Csillag lerakása random pozícióba (MakeStart) a Canvason random időben (Itt)
            if(startCounter < 1)
            {
                MakeStar();
                startCounter = rand.Next(600, 900);
            }


            foreach(var x in myCanvas.Children.OfType<Rectangle>())
            {
                //roadMarks-oknak az ismétlése, hogy mindig legyen felező vonal, mintha 'mennénk'
                if ((string)x.Tag == "roadMarks")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + speed);

                    if (Canvas.GetTop(x) > 510)
                    {
                        Canvas.SetTop(x, -152);
                    }    
                }

                //Autók lerakása random poziba random helyeken
                if ((string)x.Tag == "Car")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + speed);

                    if (Canvas.GetTop(x) > 500)
                    {
                        ChangeCars(x);
                    }

                    Rect carHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    //PowerMode azaz csillagot felvettük, azt az esetet vizsgáljuk, "elütjük"
                    if(playerHitBox.IntersectsWith(carHitBox) && powerMode == true)
                    {
                        ChangeCars(x);
                    } else if(playerHitBox.IntersectsWith(carHitBox) && powerMode == false)
                    {
                        //Ha nem vagyunk PowerMode-ban, akkor ütközünk és el is veszítjük a játékot
                        gameTimer.Stop();
                        scoreTxt.Content += " Press Enter to restart!";
                        gameOver = true;
                    }

                }

                //Csillag = PowerMode
                if((string)x.Tag == "star")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + 5);

                    Rect starHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    //Ha a playerrel ütközik, azaz felveszi
                    if (playerHitBox.IntersectsWith(starHitBox))
                    {
                        //Levesszük a képernyőről a csillagot, "felvettük"
                        itemRemover.Add(x);
                        powerMode = true;
                        powerModeCounter = 200;
                    }

                    //Ha kikerüljük a csillagot
                    if(Canvas.GetTop(x) > 400)
                    {
                        itemRemover.Add(x);
                    }
                }
            }

            //Csillag felvétele esetén PowerMode beállítása
            if(powerMode == true)
            {
                powerModeCounter -= 1;
                PowerUp();

                if (powerModeCounter < 1)
                {
                    powerMode = false;
                }
            } else
            {
                //PowerMode leállása esetén eredeti pálya dizájn visszahozása
                playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/playerimage.png"));
                myCanvas.Background = Brushes.Gray;
            }

            //Csillag eltüntetése a Canvasról ha felvettük
            foreach(Rectangle y in itemRemover)
            {
                myCanvas.Children.Remove(y);
            }

            //Pálya gyorsítása adott intervallumként
            if(score >= 10 && score < 20)
            {
                speed = 12;
            }

            if (score >= 20 && score < 30)
            {
                speed = 14;
            }

            if (score >= 30 && score < 40)
            {
                speed = 16;
            }

            if (score >= 40 && score < 50)
            {
                speed = 18;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            //Gomb lenyomás eventek
            if (e.Key == Key.Left) moveLeft = true;
            if (e.Key == Key.Right) moveRight = true;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            //Gomb felengedés eventek
            if (e.Key == Key.Left) moveLeft = false;
            if (e.Key == Key.Right) moveRight = false;

            //Ha meghaltunk restart game
            if(e.Key == Key.Enter && gameOver == true)
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            //Kezdeti értékek beállítása
            speed = 8;
            gameTimer.Start();

            moveLeft = false;
            moveRight = false;
            gameOver = false;
            powerMode = false;

            score = 0;

            scoreTxt.Content = "Survived: 0 Seconds";

            playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/playerimage.png"));
            starImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/star.png"));

            player.Fill = playerImage;

            myCanvas.Background = Brushes.Gray;

            foreach(var x in myCanvas.Children.OfType<Rectangle>())
            {
                if((string)x.Tag == "Car")
                {
                    Canvas.SetTop(x, (rand.Next(100, 400) * -1));
                    Canvas.SetLeft(x, rand.Next(0, 430));
                    ChangeCars(x);
                }

                if((string)x.Tag == "star")
                {
                    itemRemover.Add(x);
                }
            }

            itemRemover.Clear();
        }

        private void ChangeCars(Rectangle car)
        {
            //Random érték 1-6 között mert 6 féle autós képünk van
            carNum = rand.Next(1, 6);

            ImageBrush carImage = new ImageBrush();

            //Randomizált érték alapján beszúrunk egy képet a "Car" rectangle-re
            switch (carNum)
            {
                case 1:
                    carImage.ImageSource = new BitmapImage(new Uri("Pack://application:,,,/images/car1.png"));
                    break;
                case 2:
                    carImage.ImageSource = new BitmapImage(new Uri("Pack://application:,,,/images/car2.png"));
                    break;
                case 3:
                    carImage.ImageSource = new BitmapImage(new Uri("Pack://application:,,,/images/car3.png"));
                    break;
                case 4:
                    carImage.ImageSource = new BitmapImage(new Uri("Pack://application:,,,/images/car4.png"));
                    break;
                case 5:
                    carImage.ImageSource = new BitmapImage(new Uri("Pack://application:,,,/images/car5.png"));
                    break;
                case 6:
                    carImage.ImageSource = new BitmapImage(new Uri("Pack://application:,,,/images/car6.png"));
                    break;
            }

            //Feltöltjük, filleljük
            car.Fill = carImage;

            //Random pozícióba elhlelyezzük
            Canvas.SetTop(car, rand.Next(100, 400) * -1);
            Canvas.SetLeft(car, rand.Next(0, 430));

        }

        private void PowerUp()
        {
            //pálya felgyorsulása
            i += .5;

            if(i > 4)
            {
                i = 1;
            }

            //PowerMode-ban lévő player "szuper" autójának beszúrása
            switch(1)
            {
                case 1:
                    playerImage.ImageSource = new BitmapImage(new Uri("Pack://application:,,,/images/powermode1.png"));
                    break;
                case 2:
                    playerImage.ImageSource = new BitmapImage(new Uri("Pack://application:,,,/images/powermode2.png"));
                    break;
                case 3:
                    playerImage.ImageSource = new BitmapImage(new Uri("Pack://application:,,,/images/powermode3.png"));
                    break;
                case 4:
                    playerImage.ImageSource = new BitmapImage(new Uri("Pack://application:,,,/images/powermode4.png"));
                    break;
            }

            //Canvas megváltoztatása
            myCanvas.Background = Brushes.LightCoral;
        }

        private void MakeStar()
        {
            //kialakítása a Csillagnak
            Rectangle newStar = new Rectangle()
            {
                Height = 50,
                Width = 50,
                Tag = "star",
                Fill = starImage
            };

            //Random pozicíonálás
            Canvas.SetLeft(newStar, rand.Next(0, 430));
            Canvas.SetTop(newStar, rand.Next(100, 400) * -1);

            //Canvashoz adás
            myCanvas.Children.Add(newStar);
        }
    }
}
