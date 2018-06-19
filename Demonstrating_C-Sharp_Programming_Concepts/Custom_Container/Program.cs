/*
 * Author: Giedrius Kristinaitis
 * 
 * For the sake of simplicity, all classes are kept in the same file
 */

using System;
using System.IO;
using System.Text;

namespace Shapes {


    /// <summary>
    /// main program class containing the Main() method
    /// </summary>
    class Program {

        /// <summary>
        /// entry point of the program
        /// </summary>
        /// <param name="args">arguments for the program</param>
        static void Main(string[] args) {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            
            Container<Circle> circles = new Container<Circle>(50);
            Container<Polygon> polygons = new Container<Polygon>(50);

            ReadData(circles, polygons, "data.txt");
            PrintInitialDataToFile(circles, polygons, "initialData.txt");

            Menu menu = new Menu(circles, polygons, "commands.txt");
        }

        /// <summary>
        /// reads shape data from a given text file and stores it in
        /// two containers: 1 for circles and 1 for polygons
        /// </summary>
        /// <param name="circles">container to store circle data in</param>
        /// <param name="polygons">container to store polygon data in</param>
        /// <param name="fileName">file name to read from</param>
        static void ReadData(Container<Circle> circles, Container<Polygon> polygons,
            string fileName) {

            using (StreamReader reader = new StreamReader(fileName)) {
                string line = null;

                while ((line = reader.ReadLine()) != null) {
                    string name = line.Substring(0, 6);
                    string[] data = line.Substring(6).Split(' ');

                    if (data.Length == 3) {
                        circles.Add(new Circle(
                            name, double.Parse(data[0]),
                            double.Parse(data[1]), double.Parse(data[2])
                        ));
                    } else {
                        polygons.Add(CreatePolygon(name, data));
                    }
                }
            }
        }

        /// <summary>
        /// creates a polygon from a line string taken from the text file
        /// </summary>
        /// <param name="name">name for the new polygon</param>
        /// <param name="data">line string containing coordinate data</param>
        /// <returns>a new Polygon object containing given data</returns>
        static Polygon CreatePolygon(string name, string[] data) {
            double[] xCoords = new double[data.Length / 2];
            double[] yCoords = new double[data.Length / 2];

            for (int i = 0; i <= data.Length - 2; i += 2) {
				xCoords[i / 2] = double.Parse(data[i]);
                yCoords[i / 2] = double.Parse(data[i + 1]);
            }

            return new Polygon(name, xCoords, yCoords);
        }

        /// <summary>
        /// prints initial data to a text file
        /// </summary>
        /// <param name="circles">circle container to take data from</param>
        /// <param name="polygons">polygon container to take data from</param>
        /// <param name="fileName">name of the file to write to</param>
        static void PrintInitialDataToFile(Container<Circle> circles, Container<Polygon> polygons, string fileName) {
            using (StreamWriter writer = new StreamWriter(fileName)) {
                PrintCircleData(writer, circles);
                PrintPolygonData(writer, polygons);
            }
        }

        /// <summary>
        /// Prints initial circle data to a StreamWriter
        /// </summary>
        /// <param name="writer">StreamWriter to write to</param>
        /// <param name="circles">circle container to take data from</param>
        static void PrintCircleData(StreamWriter writer, Container<Circle> circles) {
            writer.WriteLine("Apskritimai:");
            PrintNStars(writer, 73, true);

            writer.WriteLine("* {0, -15} * {1, -15} * {2, -15} * {3, -15} *", "Pavadinimas",
                "Centro X", "Centro Y", "Spindulys");

            PrintNStars(writer, 73, true);

            for (int i = 0; i < circles.count; i++) {
                writer.WriteLine("* {0, -15} * {1, 15:f5} * {2, 15:f5} * {3, 15:f5} *", circles[i].name,
                    circles[i].x, circles[i].y, circles[i].radius);
            }

            PrintNStars(writer, 73, true);
            writer.WriteLine();
        }

        /// <summary>
        /// Prints initial polygon data to a StreamWriter
        /// </summary>
        /// <param name="writer">StreamWriter to write to</param>
        /// <param name="polygons">polygon container to take data from</param>
        static void PrintPolygonData(StreamWriter writer, Container<Polygon> polygons) {
            writer.WriteLine("Daugiakampiai:");
            PrintNStars(writer, 269, true);

            writer.WriteLine("* {0, -15} * {1, -247} *", "Pavadinimas", "Kampų koordinatės (x; y)");
            writer.WriteLine("* {0, -15} * {1, -22} * {2, -22} * {3, -22} * {4, -22} * {5, -22} * {6, -22} * {7, -22} * {8, -22} * {9, -22} * {10, -22} *",
                "", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10");

            PrintNStars(writer, 269, true);

            for (int i = 0; i < polygons.count; i++) {
                Polygon shape = polygons[i];

                writer.Write("* {0, -15} *", shape.name);

                for (int j = 0; j < 10; j++) {
                    if (j < shape.numOfAngles) {
                        writer.Write(" {0, 10:f5}; {1, 10:f5} *", shape.xCoordinates[j], shape.yCoordinates[j]);
                    } else {
                        writer.Write(" {0, 22} *", "-");
                    }
                }

                writer.WriteLine();
            }

            PrintNStars(writer, 269, false);
        }

        /// <summary>
        /// prints a given number of '*' characters to a StreamWriter
        /// </summary>
        /// <param name="writer">StreamWriter to write to</param>
        /// <param name="count">the number of characters</param>
        /// <param name="newLine">should there be a new line after the characters</param>
        static void PrintNStars(StreamWriter writer, int count, bool newLine) {
            for (int i = 0; i < count; i++) {
                writer.Write("*");
            }

            if (newLine) {
                writer.WriteLine();
            }
        }
    }



    /// <summary>
    /// a class that displays command menu in the console, prompts the user to enter a command,
    /// executes it (commands print results to the console and a text file)
    /// </summary>
    public class Menu: ShapeGetter {

        public Container<Circle> circles { get; set; }
        public Container<Polygon> polygons { get; set; }
        private Container<Command> commands;
        private string fileName;
        private StreamWriter fileWriter;

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="circles">a circle container containing the data required for the commands to run</param>
        /// <param name="polygons">a polygon container containing the data required for the commands to run</param>
        /// <param name="fileName">a file to which commands write execution results</param>
        public Menu(Container<Circle> circles, Container<Polygon> polygons, string fileName) {
            this.circles = circles;
            this.polygons = polygons;
            this.fileName = fileName;

            Initialize();
        }

        /// <summary>
        /// creates a StreamWriter for the commands to write, initializes commands and starts the menu
        /// </summary>
        private void Initialize() {
            using (StreamWriter fileWriter = new StreamWriter(fileName)) {
                this.fileWriter = fileWriter;

                commands = new Container<Command>(7);
                InitializeCommands(fileWriter);
                Start();
            }
        }

        /// <summary>
        /// creates all command objects
        /// </summary>
        /// <param name="fileWriter">StreamWriter for the commands to write to</param>
        private void InitializeCommands(StreamWriter fileWriter) {
            commands.Add(new Types(this, fileWriter));
            commands.Add(new MaxPerimeter(this, fileWriter));
            commands.Add(new MaxArea(this, fileWriter));
            commands.Add(new NumOfRightTriangles(this, fileWriter));
            commands.Add(new NumOfSquares(this, fileWriter));
            commands.Add(new Info(this, fileWriter));
            commands.Add(new Exit(fileWriter));
        }

        /// <summary>
        /// starts the execution of the menu
        /// </summary>
        public void Start() {
            while (true) {
                PrintMenu();

                Command command = GetCommand();

                if (command == null) {
                    Console.WriteLine("Bloga komanda!\nPaspauskite 'Enter', kad tęstumėte");
                    fileWriter.WriteLine("Bloga komanda!\nPaspauskite 'Enter', kad tęstumėte");
                    fileWriter.WriteLine();
                    Console.ReadLine();
                    Console.Clear();
                    continue;
                }

                command.Execute();

                Console.WriteLine("Paspauskite 'Enter', kad tęstumėte");
                fileWriter.WriteLine("Paspauskite 'Enter', kad tęstumėte");
                fileWriter.WriteLine();
                Console.ReadLine();
				Console.Clear();
            }
        }

        /// <summary>
        /// reads a command number from the console and returns the correct command
        /// </summary>
        /// <returns>command to execute</returns>
        private Command GetCommand() {
            Console.Write("Įveskite norimos komandos numerį: ");
            fileWriter.Write("Įveskite norimos komandos numerį: ");

            int number = int.Parse(Console.ReadLine());
            fileWriter.WriteLine("{0}", number);

            if (number >= 1 && number <= commands.count) {
                return commands[number - 1];
            }

            return null;
        }

        /// <summary>
        /// prints the command menu to the console
        /// </summary>
        public void PrintMenu() {
            PrintNStars(110, true);
            PrintNStars(1, false);
            Console.Write(" {0, -10}", "Numeris");
            PrintNStars(1, false);
            Console.Write(" {0, -20}", "Komanda");
            PrintNStars(1, false);
            Console.Write(" {0, -73}", "Aprašymas");
            PrintNStars(1, true);
            PrintNStars(110, true);

            for (int i = 0; i < commands.count; i++) {
                PrintNStars(1, false);
                Console.Write(" {0, -10}", commands[i].number);
                PrintNStars(1, false);
                Console.Write(" {0, -20}", commands[i].name);
                PrintNStars(1, false);
                Console.Write(" {0, -73}", commands[i].description);
                PrintNStars(1, true);
            }

            PrintNStars(110, true);
        }

        /// <summary>
        /// prints a given number of '*' characters to the console
        /// </summary>
        /// <param name="count">the number of characters</param>
        /// <param name="newLine">should there be a new line after the characters</param>
        private void PrintNStars(int count, bool newLine) {
            for (int i = 0; i < count; i++) {
                Console.Write("*");
            }

            if (newLine) {
                Console.WriteLine();
            }
        }

        /// <summary>
        /// gets circle container
        /// this method belongs to ShapeGetter interface which is used by the commands
        /// to get shape data
        /// </summary>
        /// <returns>circle container</returns>
		public Container<Circle> GetCircles() {
			return circles;
		}

        /// <summary>
        /// gets polygon container
        /// this method belongs to ShapeGetter interface which is used by the commands
        /// to get shape data
        /// </summary>
        /// <returns>polygon container</returns>
		public Container<Polygon> GetPolygons() {
			return polygons;
		}



        /// <summary>
        /// command class
        /// </summary>
        private abstract class Command {

            public string name { get; private set; }
            public string description { get; private set; }
            public int number { get; private set; }
			public ShapeGetter shapeGetter { get; private set; }
            public StreamWriter fileWriter { get; private set; }
            public StreamWriter console { get; private set; }

            /// <summary>
            /// class constructor
            /// </summary>
            /// <param name="name">name of the command</param>
            /// <param name="description">a brief summary of what the command does</param>
            /// <param name="number">number of the command</param>
            /// <param name="shapeGetter">ShaperGetter interface which will be used to get shape data</param>
            /// <param name="fileWriter">StreamWriter bound to a file to write results to</param>
            public Command(string name, string description, int number,
					ShapeGetter shapeGetter, StreamWriter fileWriter) {

                this.name = name;
                this.description = description;
                this.number = number;
				this.shapeGetter = shapeGetter;
                this.fileWriter = fileWriter;
            }

            /// <summary>
            /// executes the command
            /// </summary>
            public abstract void Execute();
        }



        /// <summary>
        /// command class that displays how many specific shapes there are
        /// </summary>
        private class Types: Command {

            /// <summary>
            /// class constructor
            /// </summary>
            /// <param name="shapeGetter">ShapeGetter interface to read data from</param>
            /// <param name="fileWriter">StreamWriter bound to a file to write results to</param>
			public Types(ShapeGetter shapeGetter, StreamWriter fileWriter)
				: base("KokiuIrKiek", 
                "Randa kokių ir kiek yra figūrų", 1, shapeGetter, fileWriter) { }

            /// <summary>
            /// executes the command
            /// overrides Command.Execute()
            /// </summary>
            public override void Execute() {
				int[] numberOfShapes = FindNumberOfShapes();
				PrintResults(fileWriter, numberOfShapes);
                PrintResults(numberOfShapes);
            }

            /// <summary>
            /// writes execution results to a given StreamWriter
            /// </summary>
            /// <param name="fileWriter">StreamWriter to write data to</param>
            /// <param name="numberOfShapes">an array containing how many shapes there are
            /// array index + 3 represents the number of angles the shape has and the 
            /// value at that index is the number of shapes</param>
			private void PrintResults(StreamWriter fileWriter, int[] numberOfShapes) {
				fileWriter.WriteLine("Apskritimų: {0}", shapeGetter.GetCircles().count);

				for (int i = 0; i < numberOfShapes.Length; i++) {
					if (numberOfShapes[i] != 0) {
						fileWriter.WriteLine("{0}-kampių: {1}",
							i + 3, numberOfShapes[i]);
					}
				}
			}

            /// <summary>
            /// prints execution results to the console
            /// </summary>
            /// <param name="numberOfShapes">an array containing how many shapes there are
            /// array index + 3 represents the number of angles the shape has and the 
            /// value at that index is the number of shapes</param>
            private void PrintResults(int[] numberOfShapes) {
                Console.WriteLine("Apskritimų: {0}", shapeGetter.GetCircles().count);

                for (int i = 0; i < numberOfShapes.Length; i++) {
                    if (numberOfShapes[i] != 0) {
                        Console.WriteLine("{0}-kampių: {1}",
                            i + 3, numberOfShapes[i]);
                    }
                }
            }

            /// <summary>
            /// finds the number of specific shapes. for example:
            /// 3 triangles 
            /// 1 quadrangle
            /// ....
            /// </summary>
            /// <returns>an array containing how many shapes there are
            /// array index + 3 represents the number of angles the shape has and the 
            /// value at that index is the number of shapes</returns>
            private int[] FindNumberOfShapes() {
				int[] numberOfShapes = new int[8];

				for (int i = 0; i < shapeGetter.GetPolygons().count; i++) {
					Polygon polygon = shapeGetter.GetPolygons()[i];
					numberOfShapes[polygon.numOfSides - 3]++;
				}

				return numberOfShapes;
			}
        }



        /// <summary>
        /// command that finds all shapes with highest perimeter
        /// </summary>
        private class MaxPerimeter: Command {

            /// <summary>
            /// class constructor
            /// </summary>
            /// <param name="shapeGetter">ShapeGetter interface to read data from</param>
            /// <param name="fileWriter">StreamWriter bound to a file to write results to</param>
			public MaxPerimeter(ShapeGetter shapeGetter, StreamWriter fileWriter)
				: base("MaxPerimetras", 
                "Randa didžiausią perimetrą turinčios figūros vardą ir perimetrą", 2,
				shapeGetter, fileWriter) { }

            /// <summary>
            /// executes the command
            /// overrides Command.Execute()
            /// </summary>
            public override void Execute() {
				double maxPerimeter = FindMaxPerimeter();
				Container<Shape> shapes = FindMaxPerimeterShapes(maxPerimeter);
				PrintResults(fileWriter, shapes);
                PrintResults(shapes);
            }

            /// <summary>
            /// writes execution results to a SreamWriter
            /// </summary>
            /// <param name="fileWriter">StreamWriter to write data to</param>
            /// <param name="shapes">container containing all shapes with highest perimeter</param>
			private void PrintResults(StreamWriter fileWriter, Container<Shape> shapes) {
				fileWriter.WriteLine("Didžiausią perimetrą turinti(-čios) figūra(-os):");

				for (int i = 0; i < shapes.count; i++) {
					fileWriter.WriteLine("{0} P = {1, 2:f5}", shapes[i].name,
						shapes[i].Perimeter());
				}
			}

            /// <summary>
            /// prints execution results to the console
            /// </summary>
            /// <param name="shapes">container containing all shapes with highest perimeter</param>
            private void PrintResults(Container<Shape> shapes) {
                Console.WriteLine("Didžiausią perimetrą turinti(-čios) figūra(-os):");

                for (int i = 0; i < shapes.count; i++) {
                    Console.WriteLine("{0} P = {1, 2:f5}", shapes[i].name,
                        shapes[i].Perimeter());
                }
            }

            /// <summary>
            /// finds all the shapes with highest perimeter
            /// </summary>
            /// <param name="maxPerimeter">maximum perimeter</param>
            /// <returns>container with highest perimeter shapes</returns>
            private Container<Shape> FindMaxPerimeterShapes(double maxPerimeter) {
				Container<Shape> shapes = new Container<Shape>(100);

				Container<Circle> circles = shapeGetter.GetCircles();
				Container<Polygon> polygons = shapeGetter.GetPolygons();

				for (int i = 0; i < circles.count; i++) {
					if (Math.Abs(circles[i].Perimeter() - maxPerimeter) < 0.00001) {
						shapes.Add(circles[i]);
					}
				}

				for (int i = 0; i < polygons.count; i++) {
					if (Math.Abs(polygons[i].Perimeter() - maxPerimeter) < 0.00001) {
						shapes.Add(polygons[i]);
					}
				}

				return shapes;
			}
            
            /// <summary>
            /// finds the maximum perimeter
            /// </summary>
            /// <returns>maximum perimeter as double value</returns>
			private double FindMaxPerimeter() {
				double maxPerimeter = 0;

				Container<Circle> circles = shapeGetter.GetCircles();
				Container<Polygon> polygons = shapeGetter.GetPolygons();

				for (int i = 0; i < circles.count; i++) {
					if (circles[i].Perimeter() > maxPerimeter) {
						maxPerimeter = circles[i].Perimeter();
					}
				}

				for (int i = 0; i < polygons.count; i++) {
					if (polygons[i].Perimeter() > maxPerimeter) {
						maxPerimeter = polygons[i].Perimeter();
					}
				}

				return maxPerimeter;
			}
        }



        /// <summary>
        /// command that finds all shapes with highest area
        /// </summary>
        private class MaxArea: Command {

            /// <summary>
            /// class constructor
            /// </summary>
            /// <param name="shapeGetter">ShapeGetter interface to read data from</param>
            /// <param name="fileWriter">StreamWriter bound to a file to write results to</param>
			public MaxArea(ShapeGetter shapeGetter, StreamWriter fileWriter)
				: base("MaxPlotas", 
                "Randa didžiausią plotą turinčios figūros vardą ir plotą", 3,
				shapeGetter, fileWriter) { }

            /// <summary>
            /// executes the command
            /// overrides Command.Execute()
            /// </summary>
            public override void Execute() {
				double maxArea = FindMaxArea();
				Container<Shape> shapes = FindMaxAreaShapes(maxArea);
				PrintResults(fileWriter, shapes);
                PrintResults(shapes);
            }

            /// <summary>
            /// writes execution results to a SreamWriter
            /// </summary>
            /// <param name="fileWriter">StreamWriter to write data to</param>
            /// <param name="shapes">container containing all shapes with highest area</param>
			private void PrintResults(StreamWriter fileWriter, Container<Shape> shapes) {
				fileWriter.WriteLine("Didžiausią plotą turinti(-čios) figūra(-os):");

				for (int i = 0; i < shapes.count; i++) {
					fileWriter.WriteLine("{0} S = {1, 2:f5}", shapes[i].name,
						shapes[i].Area());
				}
			}

            /// <summary>
            /// prints execution results to the console
            /// </summary>
            /// <param name="shapes">container containing all shapes with highest area</param>
            private void PrintResults(Container<Shape> shapes) {
                Console.WriteLine("Didžiausią plotą turinti(-čios) figūra(-os):");

                for (int i = 0; i < shapes.count; i++) {
                    Console.WriteLine("{0} S = {1, 2:f5}", shapes[i].name,
                        shapes[i].Area());
                }
            }

            /// <summary>
            /// finds all shapes with the highest area
            /// </summary>
            /// <param name="maxArea">maximum area</param>
            /// <returns>container of highest area shapes</returns>
            private Container<Shape> FindMaxAreaShapes(double maxArea) {
				Container<Shape> shapes = new Container<Shape>(100);

				Container<Circle> circles = shapeGetter.GetCircles();
				Container<Polygon> polygons = shapeGetter.GetPolygons();

				for (int i = 0; i < circles.count; i++) {
					if (Math.Abs(circles[i].Area() - maxArea) < 0.00001) {
						shapes.Add(circles[i]);
					}
				}

				for (int i = 0; i < polygons.count; i++) {
					if (Math.Abs(polygons[i].Area() - maxArea) < 0.00001) {
						shapes.Add(polygons[i]);
					}
				}

				return shapes;
			}

            /// <summary>
            /// finds the maximum area
            /// </summary>
            /// <returns>highest area as a double value</returns>
			private double FindMaxArea() {
				double maxArea = 0;

				Container<Circle> circles = shapeGetter.GetCircles();
				Container<Polygon> polygons = shapeGetter.GetPolygons();

				for (int i = 0; i < circles.count; i++) {
					if (circles[i].Area() > maxArea) {
						maxArea = circles[i].Area();
					}
				}

				for (int i = 0; i < polygons.count; i++) {
					if (polygons[i].Area() > maxArea) {
						maxArea = polygons[i].Area();
					}
				}

				return maxArea;
			}
        }



        /// <summary>
        /// command that finds the number of right triangles inside a circle
        /// </summary>
        private class NumOfRightTriangles: Command {

            /// <summary>
            /// class constructor
            /// </summary>
            /// <param name="shapeGetter">ShapeGetter interface to read data from</param>
            /// <param name="fileWriter">StreamWriter bound to a file to write results to</param>
			public NumOfRightTriangles(ShapeGetter shapeGetter, StreamWriter fileWriter)
				: base("KiekStacTrikampiu",
                "Randa kiek yra stačiųjų trikampių, įbrėžtų į apskritimus", 4,
				shapeGetter, fileWriter) { }

            /// <summary>
            /// executes the command
            /// overrides Command.Execute()
            /// </summary>
            public override void Execute() {
				Container<Polygon> triangles = FindAllRightTriangles();
				Container<Circle> circles = shapeGetter.GetCircles();

				int numberOfTriangles = FindTriangleInsideCircleCount(triangles);
				PrintResults(fileWriter, numberOfTriangles);
                PrintResults(numberOfTriangles);
            }

            /// <summary>
            /// writes execution results to a StreamWriter
            /// </summary>
            /// <param name="fileWriter">StreamWriter to write data to</param>
            /// <param name="numberOfTriangles">number of right triangles inside a circle</param>
			private void PrintResults(StreamWriter fileWriter, int numberOfTriangles) {
				fileWriter.WriteLine("Iš viso yra {0} trikampių, įbrėžtų į apskritimus",
					numberOfTriangles);
			}

            /// <summary>
            /// prints execution results to the console
            /// </summary>
            /// <param name="numberOfTriangles">number of right triangles inside a circle</param>
            private void PrintResults(int numberOfTriangles) {
                Console.WriteLine("Iš viso yra {0} trikampių, įbrėžtų į apskritimus",
                    numberOfTriangles);
            }

            /// <summary>
            /// finds the number of right triangles inside a circle
            /// </summary>
            /// <param name="triangles">container containing all right triangles</param>
            /// <returns>number of right triangles inside circle</returns>
            private int FindTriangleInsideCircleCount(Container<Polygon> triangles) {
				int count = 0;

                Container<Circle> circles = shapeGetter.GetCircles();

                for (int i = 0; i < circles.count; i++) {
                    for (int j = 0; j < triangles.count; j++) {
                        if (IsTriangleInsideCircle(circles[i], triangles[j])) {
                            count++;
                        }
                    }
                }

				return count;
			}

            /// <summary>
            /// checks if a triangle is inside a circle
            /// </summary>
            /// <param name="circle">Circle object to take circle data from</param>
            /// <param name="triangle">Polygon object to take triangle data from</param>
            /// <returns>true if the given triangle is inside the given circle, false otherwise</returns>
            private bool IsTriangleInsideCircle(Circle circle, Polygon triangle) {
                for (int i = 0; i < 3; i++) {
                    double distance = Utilities.DistanceBetweenPoints(
                        circle.x, triangle.xCoordinates[i], circle.y, triangle.yCoordinates[i]
                    );
                    
                    if (Math.Abs(distance - circle.radius) > 0.00001) {
                         return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// finds all right triangles
            /// </summary>
            /// <returns>container containing right triangles</returns>
			private Container<Polygon> FindAllRightTriangles() {
				Container<Polygon> triangles = new Container<Polygon>(50);

				for (int i = 0; i < shapeGetter.GetPolygons().count; i++) {
					if (shapeGetter.GetPolygons()[i].numOfSides == 3) {
                        if (IsTriangleRight(shapeGetter.GetPolygons()[i])) {
                            triangles.Add(shapeGetter.GetPolygons()[i]);
                        }
					}
				}

				return triangles;
			}

            /// <summary>
            /// checks if a triangle is right
            /// </summary>
            /// <param name="triangle">Polygon object to take traingle data from</param>
            /// <returns>true is the given triangle is right, false otherwise</returns>
            private bool IsTriangleRight(Polygon triangle) {
                double[] sides = Polygon.FindSideLengths(
                    triangle.xCoordinates, triangle.yCoordinates);

                double a = sides[0] * sides[0];
                double b = sides[1] * sides[1];
                double c = sides[2] * sides[2];

                if (Math.Abs(a + b - c) < 0.00001
                    || Math.Abs(a + c - b) < 0.00001
                    || Math.Abs(c + b - a) < 0.00001) {

                    return true;
                }

                return false;
            }
        }



        /// <summary>
        /// finds the number of squares around a circle
        /// </summary>
        private class NumOfSquares: Command {

            /// <summary>
            /// class constructor
            /// </summary>
            /// <param name="shapeGetter">ShapeGetter interface to read data from</param>
            /// <param name="fileWriter">StreamWriter bound to a file to write results to</param>
			public NumOfSquares(ShapeGetter shapeGetter, StreamWriter fileWriter)
				: base("KiekKvadratu", 
                "Randa kiek yra kvadratų, apibrėžtų apie apskritimus", 5,
				shapeGetter, fileWriter) { }

            /// <summary>
            /// executes the command
            /// overrides Command.Execute()
            /// </summary>
            public override void Execute() {
                Container<Polygon> squares = FindAllSquares();
                Container<Circle> circles = shapeGetter.GetCircles();

                int numberOfSquares = FindCircleInsideSquareCount(squares);
				PrintResults(fileWriter, numberOfSquares);
                PrintResults(numberOfSquares);
            }

            /// <summary>
            /// writes execution results to a StreamWriter
            /// </summary>
            /// <param name="fileWriter">StreamWriter to write the data to</param>
            /// <param name="numberOfSquares">number of squares around a circle</param>
			private void PrintResults(StreamWriter fileWriter, int numberOfSquares) {
				fileWriter.WriteLine("Iš viso yra {0} kvadratų, apibrėžtų apie apskritimus",
					numberOfSquares);
			}

            /// <summary>
            /// prints execution results to the console
            /// </summary>
            /// <param name="numberOfSquares">number of squares around a circle</param>
            private void PrintResults(int numberOfSquares) {
                Console.WriteLine("Iš viso yra {0} kvadratų, apibrėžtų apie apskritimus",
                    numberOfSquares);
            }

            /// <summary>
            /// finds the number of squares around a circle or, in other words, the number
            /// of circles inside a square
            /// </summary>
            /// <param name="squares">container containing all squares</param>
            /// <returns>number of squares around a circle</returns>
            private int FindCircleInsideSquareCount(Container<Polygon> squares) {
                int count = 0;

                Container<Circle> circles = shapeGetter.GetCircles();

                for (int i = 0; i < circles.count; i++) {
                    for (int j = 0; j < squares.count; j++) {
                        if (IsCircleInsideSquare(circles[i], squares[j])) {
                            count++;
                        }
                    }
                }

                return count;
            }

            /// <summary>
            /// checks if a circle is inside a square
            /// </summary>
            /// <param name="circle">Cicle object to take circle data from</param>
            /// <param name="square">Polygon object to take square data from</param>
            /// <returns>true if the given circle is inside the given square, false otherwise</returns>
            private bool IsCircleInsideSquare(Circle circle, Polygon square) {
                double middlePointX1 = (square.xCoordinates[0] + square.xCoordinates[1]) / 2;
                double middlePointY1 = (square.yCoordinates[0] + square.yCoordinates[1]) / 2;

                double middlePointX2 = (square.xCoordinates[1] + square.xCoordinates[2]) / 2;
                double middlePointY2 = (square.yCoordinates[1] + square.yCoordinates[2]) / 2;

                double distance1 = Utilities.DistanceBetweenPoints(circle.x, middlePointX1, circle.y, middlePointY1);
                double distance2 = Utilities.DistanceBetweenPoints(circle.x, middlePointX2, circle.y, middlePointY2);

                if (Math.Abs(distance1 - circle.radius) < 0.00001 
                    && Math.Abs(distance2 - circle.radius) < 0.00001) {

                    return true;
                }

                return false;
            }

            /// <summary>
            /// finds all squares
            /// </summary>
            /// <returns>container containing all squares</returns>
            private Container<Polygon> FindAllSquares() {
                Container<Polygon> squares = new Container<Polygon>(50);
                Container<Polygon> polygons = shapeGetter.GetPolygons();

                for (int i = 0; i < polygons.count; i++) {
                    Polygon shape = polygons[i];

                    if (IsSquare(shape)) {
                        squares.Add(shape);
                    }
                }

                return squares;
            }

            /// <summary>
            /// checks if a quadrangle is a square
            /// </summary>
            /// <param name="shape">Polygon object to take quadrangle data from</param>
            /// <returns>true if the given quadrangle is a square, false otherwise</returns>
            private bool IsSquare(Polygon shape) {
                double[] sides = Polygon.FindSideLengths(shape.xCoordinates, shape.yCoordinates);

                if (Polygon.IsShapeEquilateral(shape.xCoordinates, shape.yCoordinates)) {
                    double diagonal = Utilities.DistanceBetweenPoints(
                        shape.xCoordinates[0], shape.xCoordinates[2], shape.yCoordinates[0], shape.yCoordinates[2]
                    );

                    double a = sides[0] * sides[0];
                    double b = sides[1] * sides[1];
                    double c = diagonal * diagonal;

                    if (Math.Abs(a + b - c) < 0.00001
                        || Math.Abs(a + c - b) < 0.00001
                        || Math.Abs(c + b - a) < 0.00001) {

                        return true;
                    }
                }

                return false;
            }
        }



        /// <summary>
        /// command that displays information about given shape group (circles, triangles...)
        /// </summary>
        private class Info: Command {

            /// <summary>
            /// class constructor
            /// </summary>
            /// <param name="shapeGetter">ShapeGetter interface to read data from</param>
            /// <param name="fileWriter">StreamWriter bound to a file to write results to</param>
			public Info(ShapeGetter shapeGetter, StreamWriter fileWriter)
				: base("InfoFiguruGrupe",
                "Pateikia informaciją apie pageidaujamą figūrų grupę", 6,
				shapeGetter, fileWriter) { }

            /// <summary>
            /// executes the command
            /// overrides Command.Execute()
            /// </summary>
            public override void Execute() {
                Console.Write("Įveskite pageidaujamos figūrų grupės kampų skaičių (apskritimams - 0): ");
                fileWriter.Write("Įveskite pageidaujamos figūrų grupės kampų skaičių (apskritimams - 0): ");

                int numOfAngles = int.Parse(Console.ReadLine());
                fileWriter.WriteLine("{0}", numOfAngles);

                if (numOfAngles != 0 && (numOfAngles > 10 || numOfAngles < 3)) {
                    Console.WriteLine("Blogas kampų skaičius!");
                    fileWriter.WriteLine("Blogas kampų skaičius!");
                    return;
                }

                int numOfShapes = FindNumberOfShapes(numOfAngles);
                int numOfEquilateralShapes = FindNumberOfEquilateralShapes(numOfAngles);
                double perimeterSum = FindPerimeterOfAllShapes(numOfAngles);
                double areaSum = FindAreaOfAllShapes(numOfAngles);

                PrintResults(fileWriter, numOfAngles, numOfShapes, numOfEquilateralShapes, perimeterSum, areaSum);
                PrintResults(numOfAngles, numOfShapes, numOfEquilateralShapes, perimeterSum, areaSum);
            }

            /// <summary>
            /// writes execution results to a StreamWriter
            /// </summary>
            /// <param name="fileWriter">StreamWriter to write data to</param>
            /// <param name="numOfAngles">number of angles the shape group has 
            /// or, in other words, type of shape (circle, triangle...)</param>
            /// <param name="numOfShapes">number of shapes of the specified group</param>
            /// <param name="numOfEuilShapes">number of equilateral shapes</param>
            /// <param name="perimeterSum">total perimeter of the shapes of the specified group</param>
            /// <param name="areaSum">total area of the shapes of the specified group</param>
            private void PrintResults(StreamWriter fileWriter, int numOfAngles, int numOfShapes, 
					int numOfEuilShapes, double perimeterSum, double areaSum) {

				if (numOfShapes == 0) {
					fileWriter.WriteLine("{0}-kampių figūrų nėra", numOfAngles);
					return;
				}

                if (numOfAngles != 0) {
					fileWriter.WriteLine("Iš viso yra {0} {1}-kampių figūrų", numOfShapes, numOfAngles);
                } else {
					fileWriter.WriteLine("Iš viso yra {0} apskritimų", numOfShapes);
                }

				fileWriter.WriteLine("Lygiakraščių figūrų: {0}", numOfEuilShapes);
				fileWriter.WriteLine("Bendras figūrų perimetras: {0, 2:f5}", perimeterSum);
				fileWriter.WriteLine("Bendras figūrų plotas: {0, 2:f5}", areaSum);
            }

            /// <summary>
            /// prints execution results to the console
            /// </summary>
            /// <param name="numOfAngles">number of angles the shape group has 
            /// or, in other words, type of shape (circle, triangle...)</param>
            /// <param name="numOfShapes">number of shapes of the specified group</param>
            /// <param name="numOfEuilShapes">number of equilateral shapes</param>
            /// <param name="perimeterSum">total perimeter of the shapes of the specified group</param>
            /// <param name="areaSum">total area of the shapes of the specified group</param>
            private void PrintResults(int numOfAngles, int numOfShapes,
                    int numOfEuilShapes, double perimeterSum, double areaSum) {

                if (numOfShapes == 0) {
                    Console.WriteLine("{0}-kampių figūrų nėra", numOfAngles);
                    return;
                }

                if (numOfAngles != 0) {
                    Console.WriteLine("Iš viso yra {0} {1}-kampių figūrų", numOfShapes, numOfAngles);
                } else {
                    Console.WriteLine("Iš viso yra {0} apskritimų", numOfShapes);
                }

                Console.WriteLine("Lygiakraščių figūrų: {0}", numOfEuilShapes);
                Console.WriteLine("Bendras figūrų perimetras: {0, 2:f5}", perimeterSum);
                Console.WriteLine("Bendras figūrų plotas: {0, 2:f5}", areaSum);
            }

            /// <summary>
            /// finds number of shapes of the specified group
            /// </summary>
            /// <param name="numOfAngles">number of angles the shape group has (0 for circles)</param>
            /// <returns>number of shapes of the group</returns>
            private int FindNumberOfShapes(int numOfAngles) {
                int count = 0;

                if (numOfAngles == 0) {
                    return shapeGetter.GetCircles().count;
                } else {
                    Container<Polygon> shapes = shapeGetter.GetPolygons();

                    for (int i = 0; i < shapes.count; i++) {
                        if (shapes[i].numOfAngles == numOfAngles) {
                            count++;
                        }
                    }
                }

                return count;
            }

            /// <summary>
            /// finds the number of equilateral shapes of the specified group
            /// for circles returns shape count
            /// </summary>
            /// <param name="numOfAngles">number of angles the shape group has (0 for circles)</param>
            /// <returns>number of equilateral shapes</returns>
            private int FindNumberOfEquilateralShapes(int numOfAngles) {
                int count = 0;

                if (numOfAngles == 0) {
                    return shapeGetter.GetCircles().count;
                } else {
                    Container<Polygon> shapes = shapeGetter.GetPolygons();

                    for (int i = 0; i < shapes.count; i++) {
                        if (shapes[i].numOfAngles == numOfAngles) {
                            if (Polygon.IsShapeEquilateral(shapes[i].xCoordinates, 
								shapes[i].yCoordinates)) {
                                count++;
                            }
                        }
                    }
                }

                return count;
            }

            /// <summary>
            /// finds the total perimeter of shapes of the specified group
            /// </summary>
            /// <param name="numOfAngles">number of angles the shape group has (0 for circles)</param>
            /// <returns>total perimeter</returns>
            private double FindPerimeterOfAllShapes(int numOfAngles) {
                double sum = 0;

                if (numOfAngles == 0) {
                    for (int i = 0; i < shapeGetter.GetCircles().count; i++) {
                        sum += shapeGetter.GetCircles()[i].Perimeter();
                    }
                } else {
                    for (int i = 0; i < shapeGetter.GetPolygons().count; i++) {
                        if (shapeGetter.GetPolygons()[i].numOfAngles == numOfAngles) {
                            sum += shapeGetter.GetPolygons()[i].Perimeter();
                        }
                    }
                }

                return sum;
            }

            /// <summary>
            /// finds the total area of the shapes of the specified shape group
            /// </summary>
            /// <param name="numOfAngles">number of angles the shape group has (0 for circles)</param>
            /// <returns>total area</returns>
            private double FindAreaOfAllShapes(int numOfAngles) {
                double sum = 0;

                if (numOfAngles == 0) {
                    for (int i = 0; i < shapeGetter.GetCircles().count; i++) {
                        sum += shapeGetter.GetCircles()[i].Area();
                    }
                } else {
                    for (int i = 0; i < shapeGetter.GetPolygons().count; i++) {
                        if (shapeGetter.GetPolygons()[i].numOfAngles == numOfAngles) {
                            sum += shapeGetter.GetPolygons()[i].Area();
                        }
                    }
                }

                return sum;
            }
        }



        /// <summary>
        /// command that stops execution of the program
        /// </summary>
        private class Exit: Command {

            /// <summary>
            /// class constructor
            /// </summary>
            /// <param name="fileWriter">StreamWriter bound to a file to write results to</param>
            public Exit(StreamWriter fileWriter) : base("Exit", "Baigia programos darbą", 7, null, fileWriter) { }

            /// <summary>
            /// executes the command
            /// overrides Command.Execute()
            /// </summary>
            public override void Execute() {
                Console.WriteLine("Programa baigė darbą");
                fileWriter.WriteLine("Programa baigė darbą");

                fileWriter.Flush();
                fileWriter.Close();
                
                Environment.Exit(0);
            }
        }
    }



    /// <summary>
    /// ShapeGetter interface that is used by commands to get shape data
    /// </summary>
	public interface ShapeGetter {

        /// <summary>
        /// gets circle data which is provided by the class that implements this ShapeGetter interface
        /// </summary>
        /// <returns>container containing circle data</returns>
		Container<Circle> GetCircles();

        /// <summary>
        /// gets polygon data which is provided by the class that implements this ShapeGetter interface
        /// </summary>
        /// <returns>container containing polygon data</returns>
		Container<Polygon> GetPolygons();
	}



    /// <summary>
    /// container class to store data in
    /// </summary>
    /// <typeparam name="T">type of stored data</typeparam>
    public class Container<T> {

        private T[] data;
        public int capacity { get; private set; }
        public int count { get; private set; }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="capacity">maximum number of elements the container can store</param>
        public Container(int capacity) {
            this.capacity = capacity;
            data = new T[capacity];
        }

        /// <summary>
        /// add an element to the end of the container
        /// </summary>
        /// <param name="element">element to add</param>
        public void Add(T element) {
            data[count++] = element;
        }

        /// <summary>
        /// puts an element at a specific location in the container
        /// replaces current element in the specified location if exists
        /// </summary>
        /// <param name="index">position in the container</param>
        /// <param name="element">element to put</param>
        public void Put(int index, T element) {
            data[index] = element;
        }

        /// <summary>
        /// gets the element at a specific location
        /// </summary>
        /// <param name="index">position in the container</param>
        /// <returns>element at the specified position</returns>
        public T Get(int index) {
            return data[index];
        }

        /// <summary>
        /// removes an element from the container
        /// </summary>
        /// <param name="index">position of the element that is being removed</param>
        public void Remove(int index) {
            data[index] = default(T);

            for (int i = index; i < capacity - 1; i++) {
                data[i] = data[i + 1];
            }
        }

        /// <summary>
        /// overloaded [] operator for easier access of the container elements
        /// </summary>
        /// <param name="index">position in the container from which an element is returned or put to</param>
        /// <returns>element at the specified position</returns>
        public T this[int index] {
            get {
                return Get(index);
            }

            set {
                Put(index, value);
            }
        }
    }



    /// <summary>
    /// shape class
    /// </summary>
    public abstract class Shape {

        public string name { get; set; }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="name">name of the shape</param>
        public Shape(string name) {
            this.name = name;
        }

        /// <summary>
        /// finds perimeter of the shape
        /// overridden by child classes
        /// </summary>
        /// <returns>perimeter of the shape</returns>
        public abstract double Perimeter();

        /// <summary>
        /// finds area of the shape
        /// overridden by child classes
        /// </summary>
        /// <returns>area of the shape</returns>
        public abstract double Area();
    }



    /// <summary>
    /// circle class which extends Shape class
    /// </summary>
    public class Circle: Shape {

        public double radius { get; set; }
        public double x { get; set; }
        public double y { get; set; }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="name">name of the circle</param>
        /// <param name="x">center x coordinate of the circle</param>
        /// <param name="y">center y coordinate of the circle</param>
        /// <param name="radius">radius of the circle</param>
        public Circle(string name, double x, double y, double radius): base(name) {
            this.x = x;
            this.y = y;
            this.radius = radius;
        }

        /// <summary>
        /// finds perimeter of the circle
        /// overrides Shape.Perimeter()
        /// </summary>
        /// <returns>perimeter of the shape</returns>
        public override double Perimeter() {
            return 2 * Math.PI * radius;
        }

        /// <summary>
        /// finds area of the circle
        /// overrides Shape.Area()
        /// </summary>
        /// <returns>area of the shape</returns>
        public override double Area() {
            return Math.PI * radius * radius;
        }
    }



    /// <summary>
    /// polygon class which extends Shape class
    /// </summary>
    public class Polygon: Shape {

        private double[] _xCoordinates;
        public double[] xCoordinates {
            get {
                return _xCoordinates;
            }

            set {
                _xCoordinates = value;
                numOfAngles = value.Length;
                numOfSides = value.Length;
            }
        }

        public double[] yCoordinates { get; set; }
        public int numOfSides { get; private set; }
        public int numOfAngles { get; private set; }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="name">name of the polygon</param>
        /// <param name="xCoordinates">an array containing x coordinate data of all points of the shape</param>
        /// <param name="yCoordinates">an array containing y coordinate data of all points of the shape</param>
        public Polygon(string name, double[] xCoordinates, double[] yCoordinates)
            : base(name) {

            this.xCoordinates = xCoordinates;
            this.yCoordinates = yCoordinates;
        }

        /// <summary>
        /// finds area of the polygon
        /// overrides Shape.Area()
        /// </summary>
        /// <returns>area of the polygon</returns>
        public override double Area() {
            if (numOfSides == 3) {
                return FindTriangleArea(xCoordinates, yCoordinates);
            } else {
                double area = 0;

                Polygon[] triangles = DivideIntoTriangles(xCoordinates, yCoordinates);

                foreach (Polygon triangle in triangles) {
                    area += triangle.Area();
                }

                return area;
            }
        }

        /// <summary>
        /// finds perimeter of the polygon
        /// overrides Shape.Perimeter()
        /// </summary>
        /// <returns>perimeter of the polygon</returns>
        public override double Perimeter() {
            return Perimeter(xCoordinates, yCoordinates);
        }

        // ************ STATIC METHODS ************** //

        /// <summary>
        /// divides a polygon into triangles
        /// </summary>
        /// <param name="xCoords">an array containing x coordinate data of all points of a polygon</param>
        /// <param name="yCoords">an array containing y coordinate data of all points of a polygon</param>
        /// <returns>an array containing triangles</returns>
        public static Polygon[] DivideIntoTriangles(double[] xCoords, double[] yCoords) {
            Polygon[] triangles = new Polygon[xCoords.Length - 2];

            for (int i = 2; i < xCoords.Length; i++) {
                double[] xPoints = new double[3];
                double[] yPoints = new double[3];

                xPoints[0] = xCoords[0];
                yPoints[0] = yCoords[0];

                xPoints[1] = xCoords[i];
                yPoints[1] = yCoords[i];

                xPoints[2] = xCoords[i - 1];
                yPoints[2] = yCoords[i - 1];

                triangles[i - 2] = new Polygon("", xPoints, yPoints);
            }

            return triangles;
        }

        /// <summary>
        /// finds lengths of all sides of the polygon
        /// </summary>
        /// <param name="xCoords">an array containing x coordinate data of all points of a polygon</param>
        /// <param name="yCoords">an array containing y coordinate data of all points of a polygon</param>
        /// <returns>an array containing side lengths</returns>
        public static double[] FindSideLengths(double[] xCoords, double[] yCoords) {
            double[] distances = new double[xCoords.Length];

            for (int i = 0; i < xCoords.Length - 1; i++) {
                distances[i] = 
                    Utilities.DistanceBetweenPoints(xCoords[i], xCoords[i + 1], 
						yCoords[i], yCoords[i + 1]);
            }

            distances[xCoords.Length - 1] = Utilities.DistanceBetweenPoints(
                xCoords[xCoords.Length - 1], xCoords[0], yCoords[yCoords.Length - 1], yCoords[0]
            );

            return distances;
        }

        /// <summary>
        /// finds area of a triangle
        /// </summary>
        /// <param name="xCoords">an array containing x coordinate data of all points of a triangle</param>
        /// <param name="yCoords">an array containing y coordinate data of all points of a triangle</param>
        /// <returns>area of the triangle</returns>
        public static double FindTriangleArea(double[] xCoords, double[] yCoords) {
            double area = 0;

            double[] sideLengths = FindSideLengths(xCoords, yCoords);
            double halfPerimeter = Perimeter(xCoords, yCoords) / 2;

            area = Math.Sqrt(halfPerimeter * 
                (halfPerimeter - sideLengths[0]) *
                (halfPerimeter - sideLengths[1]) *
                (halfPerimeter - sideLengths[2]));

            return area;
        }

        /// <summary>
        /// finds perimeter of a polygon
        /// </summary>
        /// <param name="xCoords">an array containing x coordinate data of all points of a polygon</param>
        /// <param name="yCoords">an array containing y coordinate data of all points of a polygon</param>
        /// <returns>perimeter of the polygon</returns>
        public static double Perimeter(double[] xCoords, double[] yCoords) {
            double perimeter = 0;

            double[] distances = FindSideLengths(xCoords, yCoords);

            for (int i = 0; i < distances.Length; i++) {
                perimeter += distances[i];
            }

            return perimeter;
        }

        /// <summary>
        /// checks if a shape is equilateral
        /// </summary>
        /// <param name="xCoords">an array containing x coordinate data of all points of a polygon</param>
        /// <param name="yCoords">an array containing y coordinate data of all points of a polygon</param>
        /// <returns>true if the shape is equilateral, false otherwise</returns>
        public static bool IsShapeEquilateral(double[] xCoords, double[] yCoords) {
            double[] sides = FindSideLengths(xCoords, yCoords);
            bool equal = true;
            
            for (int j = 1; j < sides.Length; j++) {
                if (Math.Abs(sides[0] - sides[j]) > 0.00001) {
                    equal = false;
                }
            }
            
            return equal;
        }
    }



    /// <summary>
    /// helper class containing methods that may be reused for calculations
    /// </summary>
    public class Utilities {

        /// <summary>
        /// finds distance between 2 points on a 2D plane
        /// </summary>
        /// <param name="x1">x coordinate of the first point</param>
        /// <param name="x2">x coordinate of the second point</param>
        /// <param name="y1">y coordinate of the first point</param>
        /// <param name="y2">y coordinate of the second point</param>
        /// <returns>distance between the points</returns>
        public static double DistanceBetweenPoints(double x1, double x2, double y1, double y2) {
            return Math.Sqrt(
                Math.Pow(x1 - x2, 2)
                    +
                Math.Pow(y1 - y2, 2)
            );
        }
    }
}
