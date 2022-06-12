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

namespace Zadanie2_Lista2
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double wzglednyGeometrycznyXMin = 0;
        double wzglednyGeometrycznyYMin = 0;
        double wzglednyGeometrycznyXMax = 100;
        double wzglednyGeometrycznyYMax = 40;
        double aktualneX0Start = 10;
        double aktualneY0Start = 15;
        double silaPoX = 20;
        double silaPoY = 20;
        double InterwalCzasowy = 20;
        double aktualnySkladowaCzasu = 0;
        double przyrostSkladowejCzasu = 0.08;
        int silnyWiatr = 6;
        int sredniWiatr = 4;
        int lekkiWiatr = 2;
        int silaWiatru = 2;
        double silaGrawitacji = 10.2;
        double statekResp = 280;
        double statekSzybkosc = 0.5;
        int sprawnoscStatku = 2; 
        DispatcherTimer zegar = new DispatcherTimer();

        Polyline sciezkaStrzalu = new Polyline();
        Ellipse pocisk = null;
        Rectangle statek = null;
        bool gotowosc = true;
        ImageBrush wybuch = new ImageBrush();
        ImageBrush statekBrush = new ImageBrush();

        int licznikBum = 0;
        bool explozja = false;
        public MainWindow()
        {
            InitializeComponent();
            sciezkaStrzalu.Stroke = Brushes.Blue;
            myCanvas.Children.Add(sciezkaStrzalu);

            zegar.Interval = TimeSpan.FromMilliseconds(InterwalCzasowy);
            zegar.Tick += new EventHandler(Czas);
            zegar.Start();
            TworzStatek();
            wybuch.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/tenor.gif"));
            statekBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/ship.png"));
        }

            public void TworzPocisk()
            {
                pocisk = new Ellipse();
                pocisk.Width = 8;
                pocisk.Height = 16;

                RadialGradientBrush radialGradient = new RadialGradientBrush();
                radialGradient.GradientOrigin = new Point(0.5, 0.5);
                radialGradient.Center = new Point(0.5, 0.5);
                radialGradient.RadiusX = 0.5;
                radialGradient.RadiusY = 0.5;
                radialGradient.GradientStops.Add(new GradientStop(Colors.Orange, 0.0));
                radialGradient.GradientStops.Add(new GradientStop(Colors.Red, 0.25));
                radialGradient.GradientStops.Add(new GradientStop(Colors.Black, 0.75));
                radialGradient.GradientStops.Add(new GradientStop(Colors.Black, 1.0));
                pocisk.Fill = radialGradient;
                myCanvas.Children.Add(pocisk);

            }
       
        public void TworzStatek()
        {
            statek = new Rectangle();
            statek.Width = 25;
            statek.Height = 50;
            statekResp = 280;
            statek.Stretch = Stretch.Fill;
            statek.Fill = statekBrush;
            myCanvas.Children.Add(statek);
            Canvas.SetLeft(statek, 280);
            Canvas.SetTop(statek, 250);
        }

        private void przyciskStrzalClick(object sender, RoutedEventArgs e)
        {
            if (!gotowosc) return;
            else gotowosc = false;
            if (pocisk == null) TworzPocisk();
            else myCanvas.Children.Remove(pocisk);

            aktualnySkladowaCzasu = 0;
            sciezkaStrzalu = new Polyline();
            sciezkaStrzalu.Stroke = Brushes.Blue;
            myCanvas.Children.Add(sciezkaStrzalu);

            double zasiegXMax = 2 * silaPoX * silaPoY / silaGrawitacji;
            double zasiegYMax = 0.5 * silaPoY * silaPoY / silaGrawitacji;
            double poziomMax = Math.Round(aktualneX0Start + zasiegXMax, 0);
            double pionMax = Math.Round(aktualneY0Start + zasiegYMax, 0);
        }


        private bool koliza()
        {
            Rect r1 = new Rect(Canvas.GetLeft(pocisk), Canvas.GetTop(pocisk),pocisk.Width,pocisk.Height);
            Rect r2 = new Rect(Canvas.GetLeft(statek), Canvas.GetTop(statek), statek.Width+10, statek.Height+10);
            return r1.IntersectsWith(r2);

        }
        private void czasPocisku()
        {
            double x = aktualneX0Start + (silaPoX - silaWiatru) * aktualnySkladowaCzasu;
            double y = aktualneY0Start + (silaPoY - silaWiatru) * aktualnySkladowaCzasu - 0.5 * silaGrawitacji * aktualnySkladowaCzasu * aktualnySkladowaCzasu;
            if (y >= aktualneY0Start-10)
            {
                Canvas.SetLeft(pocisk, NormalizujPoX(x));
                Canvas.SetTop(pocisk, NormalizujPoY(y));
                sciezkaStrzalu.Points.Add(new Point( NormalizujPoX(x), NormalizujPoY(y)));
            }
            else
            {
                myCanvas.Children.Remove(pocisk);
                pocisk = null;
                myCanvas.Children.Remove(sciezkaStrzalu);
                gotowosc = true;
                return;
            }
            aktualnySkladowaCzasu += przyrostSkladowejCzasu;
        }

        private void czasStatku()
        {
            
                statekResp -= statekSzybkosc;
                Canvas.SetLeft(statek, statekResp);
            if (Canvas.GetLeft(statek) < 40)
            {
                MessageBox.Show("Przegrales");
                zegar.Stop();
            }
        }


        private void Czas(object sender, EventArgs e)
        {
            
            if (pocisk != null) czasPocisku();
            if (statek != null) czasStatku();
            else
            {
                TworzStatek();
            }
            if (statek != null && pocisk != null)
            {

                if (koliza())
                {
                    Console.WriteLine("Trafiony");
                    sprawnoscStatku--;
                    myCanvas.Children.Remove(pocisk);
                    myCanvas.Children.Remove(sciezkaStrzalu);
                    pocisk = null;
                    gotowosc = true;
                    if (sprawnoscStatku == 0)
                    {
                        statek.Fill = wybuch;
                        explozja = true;
                        int punkty = int.Parse(punktacja.Text);
                        punkty++;
                        punktacja.Text = punkty.ToString();
                        statekSzybkosc = 0.5;
                        sprawnoscStatku = 2;
                        licznikBum = 0;
                    }
                    else
                    {
                        
                        statekSzybkosc = 0.2;
                    }
                }
            }

            if (explozja == true && licznikBum>=5)
            {
                 myCanvas.Children.Remove(statek);
                statek = null;
                licznikBum = 0;
                explozja = false;
                SolidColorBrush brush = new SolidColorBrush();
                Random rnd = new Random();
                int i =rnd.Next(0, 3);
                if(statekSzybkosc < 1.5) statekSzybkosc += 0.1;
                switch (i)
                {
                    case 0: silaWiatru = lekkiWiatr;  wiatr.Text = "Lekki Wiatr";
                        brush.Color = Color.FromRgb(0, 255, 0);
                        Strzala.Fill = brush; 
                        break;
                    case 1:
                        silaWiatru = sredniWiatr; wiatr.Text ="Średni Wiatr";
                        brush.Color = Color.FromRgb(0, 0, 255);
                        Strzala.Fill = brush; 
                        break;
                    case 2:
                        silaWiatru = silnyWiatr; wiatr.Text = "Silny Wiatr";
                        brush.Color = Color.FromRgb(255, 0, 0);
                        Strzala.Fill = brush; 
                        break;
                }
            

            }
            else licznikBum++;
     
        }
        private double NormalizujPoX(double x)
        {
            double result = (x - wzglednyGeometrycznyXMin) * myCanvas.Width / (wzglednyGeometrycznyXMax - wzglednyGeometrycznyXMin);
            return result;
        }
        private double NormalizujPoY(double y)
        {
            double result = myCanvas.Height - (y - wzglednyGeometrycznyYMin)* myCanvas.Height/(wzglednyGeometrycznyYMax - wzglednyGeometrycznyYMin);
            return result;
        }
        private void przyciskZamknijClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                silaPoX = double.Parse(silaX.Text);
            }
            catch (System.FormatException er) { }
           
        }

        private void silaY_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            { 
            silaPoY = double.Parse(silaY.Text);
            }
            catch (System.FormatException er) { }
        }
    }
}
