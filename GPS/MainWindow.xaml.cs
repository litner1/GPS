using System;
using System.Collections.Generic;
using System.IO;
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
using System.Drawing;
using Point = System.Windows.Point;
using Brushes = System.Windows.Media.Brushes;
using System.Windows.Threading;

namespace GPS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //zmienne

        Dijkstra alg = new Dijkstra();//dijkstra
        List<Cross> crosses = new List<Cross>();//lista wszystkich skrzyzowan
        List<int> route = new List<int>();//droga od startu do końca
        Cross tmp = new Cross();//skrzyżowanie pomocnicze
        Bitmap bitmap = new Bitmap(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Resources\crosses.png");//mapa
        int[] parents;//kolejni rodzice od punktu końcowego do startu
        int startVertex = 100;//start
        int endVertex = 0;//koniec
        System.Windows.Point mouse = new System.Windows.Point();//punkt zaznaczony myszką
        List<Line> roads = new List<Line>();//droga za pomoca linii
        double angle;//kąt linii
        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();//wątek samochodu
        List<Ellipse> stops = new List<Ellipse>();//ulice jednokierunkowe
        List<List<int>> traffics = new List<List<int>>();//korki

        //konwertery
        public static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }
        public static BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
        //porownanie kolorow
        bool Comparator(System.Drawing.Color x, System.Drawing.Color y)
        {
            int tmp = 40;
            if (x.B <= y.B + tmp && x.B >= y.B - tmp &&
                x.G <= y.G + tmp && x.B >= y.B - tmp &&
                x.R <= y.R + tmp && x.R >= y.R - tmp)
                return true;
            return false;
        }
        //kolorowanie pikseli
        public static void coloring(Bitmap bitmap, int x, int y, System.Drawing.Color color)
        {
            int scale = 15;
            for (int i = -scale; i <= scale; i++)
            {
                for (int j = -scale; j <= scale; j++)
                {
                    if (x + i <= 0 || x + i >= bitmap.Width)
                        continue;
                    if (y + j <= 0 || y + j >= bitmap.Height)
                        continue;
                    bitmap.SetPixel(x + i, y + j, color);
                }
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            WriteableBitmap bitmapTmp = new WriteableBitmap(CreateBitmapSourceFromBitmap(bitmap));
            map.Source = bitmapTmp;
            
            
            //dodanie czarnych punktow jako sktzyzowania

            int counter = 0;
            for (int i = 2; i < bitmap.Width - 2; i++)
            {
                for (int j = 2; j < bitmap.Height - 2; j++)
                {
                    if (Comparator(bitmap.GetPixel(i, j), System.Drawing.Color.Black) && Comparator(bitmap.GetPixel(i + 4, j + 4), System.Drawing.Color.Black))
                    {
                        Cross tmp = new Cross();
                        tmp.x = i + 2;
                        tmp.y = j + 2;
                        tmp.index = counter;
                        counter++;
                        crosses.Add(tmp);
                    }
                }
            }

            //wczytanie z pliku
            string line;
            List<string> lines = new List<string>();
            System.IO.StreamReader file = new System.IO.StreamReader(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Resources\crosses.txt");
            while ((line = file.ReadLine()) != null)
            {
                if (line == "")
                    continue;
                lines.Add(line);
            }

            //zapisanie do crosses
            for (int i = 0; i < crosses.Count; i++)
            {
                string[] vs = lines[i].Split('\t');
                if (vs[0] != "")
                {
                    for (int j = 1; j < vs.Length; j++)
                        crosses[i].neighbours.Add(int.Parse(vs[j]));
                }
            }

            //sortowanie
            for (int i = 0; i < crosses.Count; i++)
                crosses[i].neighbours.Sort();



            //dodanie odleglosci i predkosci
            for (int i = 0; i < crosses.Count; i++)
            {
                for (int j = 0; j < crosses[i].neighbours.Count; j++)
                {
                    crosses[i].distance.Add(Math.Sqrt(Math.Pow((crosses[i].x - crosses[crosses[i].neighbours[j]].x), 2)
                        + Math.Pow((crosses[i].y - crosses[crosses[i].neighbours[j]].y), 2)));
                }
            }
            StartVertex.Text = startVertex.ToString();
            EndVertex.Text = endVertex.ToString();

            //utworzenie listy linii i stopow
            roads.Add(line00);
            roads.Add(line01);
            roads.Add(line02);
            roads.Add(line03);
            roads.Add(line04);
            roads.Add(line05);
            roads.Add(line06);
            roads.Add(line07);
            roads.Add(line08);
            roads.Add(line09);
            roads.Add(line10);
            roads.Add(line11);
            roads.Add(line12);
            roads.Add(line13);
            roads.Add(line14);
            roads.Add(line15);
            roads.Add(line16);
            roads.Add(line17);
            roads.Add(line18);
            roads.Add(line19);
            roads.Add(line20);
            roads.Add(line21);
            roads.Add(line22);
            roads.Add(line23);
            roads.Add(line24);
            roads.Add(line25);
            roads.Add(line26);
            roads.Add(line27);
            roads.Add(line28);
            roads.Add(line29);

            stops.Add(stop00);
            stops.Add(stop01);
            stops.Add(stop02);
            stops.Add(stop03);
            stops.Add(stop04);
            stops.Add(stop05);
            stops.Add(stop06);
            stops.Add(stop07);
            stops.Add(stop08);
            stops.Add(stop09);
            stops.Add(stop10);
            stops.Add(stop11);
            stops.Add(stop12);
            stops.Add(stop13);
            stops.Add(stop14);
            stops.Add(stop15);
            stops.Add(stop16);
            stops.Add(stop17);
            stops.Add(stop18);
            stops.Add(stop19);
            stops.Add(stop20);
            stops.Add(stop21);
            stops.Add(stop22);
            stops.Add(stop23);
            stops.Add(stop24);
            stops.Add(stop25);
            stops.Add(stop26);
            stops.Add(stop27);
            stops.Add(stop28);
            stops.Add(stop29);
            stops.Add(stop30);
            stops.Add(stop31);
            stops.Add(stop32);
            stops.Add(stop33);
            stops.Add(stop34);
            stops.Add(stop35);
            stops.Add(stop36);
            stops.Add(stop37);
            stops.Add(stop38);
            stops.Add(stop39);
            stops.Add(stop40);
            stops.Add(stop41);
            stops.Add(stop42);
            stops.Add(stop43);
            stops.Add(stop44);
            stops.Add(stop45);
            stops.Add(stop46);
            stops.Add(stop47);
            stops.Add(stop48);
            stops.Add(stop49);

            //osobny wątek do poruszania samochodem      
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);

            //wyłączanie guzików
            test3.IsEnabled = false;
            test4.IsEnabled = false;

        }

        //dijkstra od (a,b)   
        //macierz incydencji
        double[,] setGraph(List<List<int>> traffics)
        {

            
            //losowanie prędkości dróg
            var rnd = new Random(0);
            var rnd2 = new Random();

            if (rnd2.Next(3)==0 || crosses[0].velocity.Count == 0)
            {

                for (int i = 0; i < crosses.Count; i++)
                    crosses[i].velocity.Clear();

                for (int i = 0; i < crosses.Count; i++)
                {
                    for (int j = 0; j < crosses[i].neighbours.Count; j++)
                    {
                        int x = rnd.Next(0, 14);
                        if (x == 0)
                            crosses[i].velocity.Add(0);
                        else
                        {
                            int vel = rnd2.Next(3);
                            crosses[i].velocity.Add(30 + 10 * vel);
                        }

                    }
                }
            }



            //korki
           for(int i = 0; i < traffics.Count; i++)
           {
                for (int j = 0; j < crosses[traffics[i][0]].neighbours.Count; j++)
                {
                    if (crosses[traffics[i][0]].neighbours[j] == traffics[i][1]) 
                    {
                        crosses[traffics[i][0]].velocity[j] = 10;
                        for (int k = 0; k < crosses[traffics[i][1]].neighbours.Count; k++) 
                        {
                            if (crosses[traffics[i][1]].neighbours[k] == traffics[i][0])
                            {
                                crosses[traffics[i][1]].velocity[k] = 10;
                                break;
                            }
                        }
                        break;
                    }
                }
           }


            var graph = new double[crosses.Count, crosses.Count];
            for (int i = 0; i < crosses.Count; i++)
            {
                for (int j = 0; j < crosses.Count; j++)
                {
                    for (int k = 0; k < crosses[i].neighbours.Count; k++)
                    {
                        if (crosses[i].neighbours[k] == crosses[j].index)
                        {
                            graph[i, j] = crosses[i].distance[k] * 1 / crosses[i].velocity[k];
                            break;
                        }
                        else
                            graph[i, j] = 0;
                    }
                }
            }

            return graph;
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            var graph = setGraph(traffics);//macierz incydencji

            parents = alg.dijkstra(graph, startVertex);
            List<int> tmp = new List<int>();
            int index = endVertex;
            tmp.Add(index);
            while (index != startVertex)
            {
                index = parents[index];
                tmp.Add(index);
            }
            route = tmp;

            //pokazanie drogi
            bitmap = new Bitmap(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Resources\crosses.png");
            for (int i = 0; i < route.Count; i++)
            {
                coloring(bitmap, crosses[route[i]].x, crosses[route[i]].y, System.Drawing.Color.BlueViolet);
            }
            coloring(bitmap, crosses[startVertex].x, crosses[startVertex].y, System.Drawing.Color.OrangeRed);
            coloring(bitmap, crosses[endVertex].x, crosses[endVertex].y, System.Drawing.Color.Orange);
            WriteableBitmap bitmapTmp = new WriteableBitmap(CreateBitmapSourceFromBitmap(bitmap));
            map.Source = bitmapTmp;

            for (int i = 0; i < roads.Count; i++)//ukrycie wcześniej zrobionych dróg
                roads[i].StrokeThickness = 0;



            //ulice jednokierunkowe
            bitmap = new Bitmap(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Resources\crosses.png");
            int count= 0;
            for (int i = 0; i < crosses.Count; i++)
            {
                for (int j = 0; j < crosses[i].neighbours.Count; j++)
                {
                    if (crosses[i].velocity[j] == 0)
                    {
                        int scale = 3;
                        double x1 = (scale * crosses[i].x + crosses[crosses[i].neighbours[j]].x) / (scale + 1) * map.ActualWidth / bitmap.Width;
                        double y1 = (scale * crosses[i].y + crosses[crosses[i].neighbours[j]].y) / (scale + 1) * map.ActualHeight / bitmap.Height;
                        x1 -= stops[count].Width / 2;
                        y1 -= stops[count].Height / 2;

                        stops[count].Stroke = System.Windows.Media.Brushes.Red;
                        stops[count].StrokeThickness = 3;
                        stops[count].Fill = System.Windows.Media.Brushes.White;
                        stops[count].Margin = new Thickness(x1, y1, 0, 0);

                        count++;
                    }
                }
            }
            bitmapTmp = new WriteableBitmap(CreateBitmapSourceFromBitmap(bitmap));
            map.Source = bitmapTmp;

            counter = 0;//wyzerowanie ruchu pojazdu
            counter2 = 0;

            double ratio = Math.Pow(Math.Pow(crosses[13].x - crosses[106].x, 2) + Math.Pow(crosses[13].y - crosses[106].y, 2), 0.5);
            ratio = 7000 / ratio; //5.27 troche za malo
            double distance = 0;
            for (int i = 0; i < route.Count - 1; i++)
            {
                for (int j = 0; j < crosses[route[i]].neighbours.Count; j++)
                {
                    if (route[i + 1] == crosses[route[i]].neighbours[j])
                    {
                        distance += crosses[route[i]].distance[j];
                        break;
                    }
                }
            }

            Remaining_way.Content = ((int)(distance * ratio) / 50) * 50;
            Remaining_way.Content += " metrów";

            //buttony
            test3.IsEnabled = true;
        }

      
        private void Test2_Click(object sender, RoutedEventArgs e)
        {
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Resources\help.txt";
            string[] x = File.ReadAllLines(path, Encoding.GetEncoding(1250));
            string tmp = null;
            for (int i = 0; i < x.Length; i++)
                tmp += x[i] + "\n";


            MessageBox.Show(tmp);
        }

        //rysowanie łamanej od a do b
        private void Test3_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < route.Count - 1; i++)
            {
                roads[i].X1 = crosses[route[i]].x * map.ActualWidth / bitmap.Width;
                roads[i].Y1 = crosses[route[i]].y * map.ActualHeight / bitmap.Height;
                roads[i].X2 = crosses[route[i + 1]].x * map.ActualWidth / bitmap.Width;
                roads[i].Y2 = crosses[route[i + 1]].y * map.ActualHeight / bitmap.Height;
                roads[i].Stroke = System.Windows.Media.Brushes.Blue;
                roads[i].StrokeThickness = 20;
            }

            car.Stroke = System.Windows.Media.Brushes.Black;
            car.StrokeThickness = 5;
            car.Fill = System.Windows.Media.Brushes.Yellow;
            route.Reverse();

            var x1 = crosses[route[0]].x * map.ActualWidth / bitmap.Width;
            var x2 = crosses[route[0]].y * map.ActualHeight / bitmap.Height;
            x1 -= car.Width / 2;
            x2 -= car.Height / 2;

            var place = new Thickness(x1, x2, 0, 0);
            car.Margin = place;

            test3.IsEnabled = false;
            test4.IsEnabled = true;
        }

        //ruch pojazdu
        private void Test4_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Start();
        }

        int counter = 0;
        int counter2 = 0;
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //pokazanie prędkości
            for (int i = 0; i < crosses[route[counter]].neighbours.Count; i++)
            {
                if (route[counter + 1] == crosses[route[counter]].neighbours[i])
                {
                    Road_velocity.Content = crosses[route[counter]].velocity[i].ToString();
                    Road_velocity.Content += " km/h";
                    break;
                }
            }

            //dystans referencyjny i przebyty
            //72-87 - 1.3km
            double ratio = Math.Pow(Math.Pow(crosses[13].x - crosses[106].x, 2) + Math.Pow(crosses[13].y - crosses[106].y, 2), 0.5);
            ratio = 7000 / ratio; //5.27 troche za malo
            double distance = 0;
            for (int i = 0; i < route.Count - 1; i++) 
            {
                for(int j=0; j < crosses[route[i]].neighbours.Count; j++)
                {
                    if (route[i + 1] == crosses[route[i]].neighbours[j])
                    {
                        distance += crosses[route[i]].distance[j];
                        break;
                    }
                }
            }
            double traveled_x = crosses[route[0]].x - (car.Margin.Left + car.Width / 2) / map.ActualWidth * (double)bitmap.Width;
            double traveled_y = crosses[route[0]].y - (car.Margin.Top + car.Height / 2) / map.ActualHeight * (double)bitmap.Height;
            double traveled_road = Math.Pow(traveled_x * traveled_x + traveled_y * traveled_y, 0.5);
            distance -= traveled_road;

            Remaining_way.Content = ((int)(distance * ratio) / 50) * 50;
            Remaining_way.Content += " metrów";


            //matematyka kąta
            double x0 = crosses[route[counter]].x * map.ActualWidth / (double)bitmap.Width;
            double x1 = crosses[route[counter + 1]].x * map.ActualWidth / (double)bitmap.Width;
            double y0 = crosses[route[counter]].y * map.ActualHeight / (double)bitmap.Height;
            double y1 = crosses[route[counter + 1]].y * map.ActualHeight / (double)bitmap.Height;
            double x = x1 - x0;
            double y = y1 - y0;
            angle = y / x;
            if (y < 0 && x > 0)
                angle = -Math.Abs(angle);
            if (y < 0 && x < 0)
                angle = -Math.Abs(angle);
            if (y > 0 && x < 0)
                angle = Math.Abs(angle);



            var newPlace = car.Margin;
            if (crosses[route[counter + 1]].x < crosses[route[counter]].x)
                newPlace.Left--;
            else
                newPlace.Left++;
            newPlace.Top = newPlace.Top + angle;
            car.Margin = new Thickness(newPlace.Left, newPlace.Top, 0, 0);

            //kasowanie przejechanej drogi
            if (x > 0)
                roads[route.Count - counter - 2].X2++;
            else
                roads[route.Count - counter - 2].X2--;
            roads[route.Count - counter - 2].Y2 += angle;

            double Y = car.Margin.Top * bitmap.Height / map.ActualHeight;
            double X = car.Margin.Left * bitmap.Width / map.ActualWidth;

            counter2++;
            if (counter2 == (int)(Math.Abs(crosses[route[counter + 1]].x - crosses[route[counter]].x) * map.ActualWidth / bitmap.Width))
            {
                startVertex = route[counter + 1];
                StartVertex.Text = startVertex.ToString();

                //usuwanie drogi
                roads[route.Count - counter - 2].StrokeThickness = 0;
                counter2 = 0;
                counter++;

                test.IsEnabled = true;
                test4.IsEnabled = false;

                if (counter == route.Count - 1)
                {
                    test4.IsEnabled = false;
                    dispatcherTimer.Stop();
                    return;
                }

                dispatcherTimer.Stop();
            }
        }

        //pokazanie wezla i sasiadow
        private void ShowVertex(object sender, TextChangedEventArgs e)
        {
            if (showVertex.Text != "")
            {
                if (int.Parse(showVertex.Text) > 106)
                {
                    bitmap = new Bitmap(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Resources\crosses.png");
                }
                else
                {
                    int X = crosses[int.Parse(showVertex.Text)].x;
                    int Y = crosses[int.Parse(showVertex.Text)].y;
                    bitmap = new Bitmap(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Resources\crosses.png");
                    coloring(bitmap, X, Y, System.Drawing.Color.MidnightBlue);
                    for (int i = 0; i < crosses[int.Parse(showVertex.Text)].neighbours.Count; i++)
                    {
                        coloring(bitmap, crosses[crosses[int.Parse(showVertex.Text)].neighbours[i]].x,
                            crosses[crosses[int.Parse(showVertex.Text)].neighbours[i]].y, System.Drawing.Color.OrangeRed);
                    }
                }
                WriteableBitmap bitmapTmp = new WriteableBitmap(CreateBitmapSourceFromBitmap(bitmap));
                map.Source = bitmapTmp;
            }
        }


        //punkt startowy
        private void StartVertex_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (StartVertex.Text != "")
                startVertex = int.Parse(StartVertex.Text);
        }

        //punkt końcowy
        private void EndVertex_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EndVertex.Text != "")
                endVertex = int.Parse(EndVertex.Text);
        }

        //zapisuje współrzędne kliknięcia oraz pokazuje numer skrzyżowania z sąsiadami
        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BitmapSource bitmapTmp = (BitmapSource)map.Source;
            Bitmap bitmap = new Bitmap(BitmapFromSource(bitmapTmp));
            var point = Mouse.GetPosition(map);
            // int x = point.X/MainWindow.ActualWidthProperty
            double x = column.ActualWidth;
            double y = grid.ActualHeight;
            //  if (a.IsEmpty)
            mouse.X = (int)point.X;
            mouse.Y = (int)point.Y;

            for (int i = 0; i < crosses.Count; i++)
            {
                if (Math.Abs(crosses[i].x - mouse.X * bitmap.Width / map.ActualWidth) < 5
                    && Math.Abs(crosses[i].y - mouse.Y * bitmap.Height / map.ActualHeight) < 5)
                {
                    showVertex.Text = i.ToString();
                    break;
                }
            }
        }


        private void Map_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            BitmapSource bitmapTmp = (BitmapSource)map.Source;
            Bitmap bitmap = new Bitmap(BitmapFromSource(bitmapTmp));
            var point = Mouse.GetPosition(map);
            // int x = point.X/MainWindow.ActualWidthProperty
            double x = column.ActualWidth;
            double y = grid.ActualHeight;
            //  if (a.IsEmpty)
            mouse.X = (int)point.X;
            mouse.Y = (int)point.Y;

            //tworzenie korków na drodze
            for(int i = 0; i < roads.Count; i++)
            {
                if (roads[i] == sender)
                {

                    roads[i].Stroke = System.Windows.Media.Brushes.Orange;
                    double x1 = roads[i].X1 / map.ActualWidth * bitmap.Width;
                    double x2 = roads[i].X2 / map.ActualWidth * bitmap.Width;
                    double y1 = roads[i].Y1 / map.ActualHeight * bitmap.Height;
                    double y2 = roads[i].Y2 / map.ActualHeight * bitmap.Height;

                    int a = 0;
                    int b = 0;

                    for( int j = 0; j < crosses.Count; j++)
                    {
                        if (Math.Abs(crosses[j].x - x1) < 5 && Math.Abs(crosses[j].y - y1) < 5)
                            a = j;
                        if (Math.Abs(crosses[j].x - x2) < 5 && Math.Abs(crosses[j].y - y2) < 5)
                            b = j;

                    }
                    List<int> tmp = new List<int>();
                    tmp.Add(a);
                    tmp.Add(b);
                    traffics.Add(tmp);
                }
            }
        }

    }
}
