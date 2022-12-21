using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace sudoku_solver
{
    /*
    Tøída reprezentuje algorimus pro vyhodnocení sudoku
    Tøída obsahuje:
     - size - velikost sudoku = 9
     - numbers - herní pole
     - inputFilePath - název vstupního souboru
     - outputFilePath - název výstupního souboru
    */
    class Solver
    {
        private int size = 9;
        private int[][] numbers;
        public string inputFilePath { get; set; } = "";
        public string outputFilePath { get; set; } = "sudoku_solved.txt";

        public Solver()
        {
            this.numbers = new int[size][];
            for (int i = 0; i < this.numbers.Length; i++)
            {
                this.numbers[i] = new int[size];
            }
        }

        /*
        funkce zkontroluje jestli jsou vstupní data validní,
        pokud ano, tak se zavolá algorimus pro øešení sudoku
         */
        public bool solve()
        {
            if (this.checkValidity() == true)
            {
                Debug.WriteLine("Solving sudoku...");
                return this.solveSudoku();
            }
            else
            {
                Debug.WriteLine("Not valid input...");
                return false;
            }
        }

        /*
        Algorimus pro øešení sudoku
        - backtracking

        1. nejpvve se hledá volné místo v hracím poli,
           jakmile se toto místo najde, tak se uloží do promìnných
           row, column
        2. pokud se projde pøes celé pole bez toho aniž by našlo prázdné místo,
           tak algoritmus vrátí true a sudoku bylo vyøešeno
        3. pokud sudoku nebylo doposud vyøešeno, tak se pokraèuje dále,
           kde se pro danou volnou pozici najdou všechny èísla, které jdu zapsat na danou pozici
           Tyto hodnoty jsou uloženy v SortedSet safeNumbers
        4. zaène se iterovat pøes list safeNumbers.
           Vezme se první èíslo z listu a vloží se na danou volnou pozici
        5. Násldenì se v podmínce rekurzivnì zavolá funkce solveSudoku,
           kde se opìt hledá volné místo a proces se tak opakuje
        6. pokud se ovšem nalezne volné místo, kde nejde vložit žádné èíslo
           tak funkce vrací false a prùbìh algorimu se vrátí o jedno zavolání funkce
           sudokuSolve a nastaví se, pøedešlo pozici nula a zkusí se zde vložit nové èíslo.
        7. tento proces probíhá do té doby do se na dané místo nanastaví ta správná hodnota,
           která nebude nikde s nièím kolidovat
        8. pokud se funkce vrátí úplnì na zaèátek procesu, to znamená, že vše co funkce do
           herního pole zapsala bylo také smazáno a již nejsou další hodnoty které by se daly vyzkoušet,
           algoritmus konèí a vrací false
         */
        private bool solveSudoku()
        {
            int row = -1;
            int column = -1;
            bool isEmpty = true;

            for (int y = 0; y < this.size; y++)
            {
                for (int x = 0; x < this.size; x++)
                {
                    if (this.numbers[y][x] == 0)
                    {
                        row = y;
                        column = x;

                        isEmpty = false;
                        break;
                    }
                }

                if (isEmpty == false)
                {
                    break;
                }
            }

            if (isEmpty == true)
            {
                return true;
            }

            var safeNumbers = this.getUnusedNumbers(row, column);
            foreach (var safeNum in safeNumbers)
            {
                this.set(row, column, safeNum);
                if (this.solveSudoku())
                {
                    return true;
                }
                else
                {
                    this.set(row, column, 0);
                }
            }

            return false;
        }

        /*--------------------------------------------------------------*/

        /*
        funkce kontroluje, jestli jsou vstupní data pro algoritmus validní
         */
        private bool checkValidity()
        {
            for (int i = 0; i < this.size; i++)
            {
                if (this.checkLine(i) == false)
                    return false;
                if (this.checkColumn(i) == false)
                    return false;
            }

            for (int i = 0; i <= 6; i += 3)
            {
                for (int j = 0; j <= 6; j += 3)
                {
                    if (this.checkRect(i, j) == false)
                        return false;
                }
            }

            return true;
        }

        /*
        funkce kontroluje jestli se v daném øádku, v herním poli, nenachází stejné èísla
         */
        private bool checkLine(int id)
        {
            if (id < 0 || id >= this.size)
                return false;

            var line = new SortedSet<int>();
            for (int i = 0; i < this.size; i++)
            {
                if (this.numbers[id][i] == 0)
                {
                    continue;
                }

                if (line.Contains(this.numbers[id][i]) == true)
                {
                    return false;
                }
                else
                {
                    line.Add(this.numbers[id][i]);
                }
            }

            return true;
        }

        /*
        funkce kontroluje jestli se v daném sloupci, v herním poli, nenachází stejné èísla
         */
        private bool checkColumn(int id)
        {
            if (id < 0 || id >= this.size)
                return false;

            var line = new SortedSet<int>();
            for (int i = 0; i < this.size; i++)
            {
                if (this.numbers[i][id] == 0)
                {
                    continue;
                }

                if (line.Contains(this.numbers[i][id]) == true)
                {
                    return false;
                }
                else
                {
                    line.Add(this.numbers[i][id]);
                }
            }

            return true;
        }

        /*
        funkce kontroluje jestli se v daném ètverci, v herním poli, nenachází stejné èísla
         */
        private bool checkRect(int y, int x)
        {
            if (x < 0 || y < 0 || x >= 9 || y >= 9)
                return false;

            var line = new SortedSet<int>();
            int minY = y / 3;
            minY *= 3;
            for (int i = minY; i < minY + 3; i++)
            {
                int minX = x / 3;
                minX *= 3;
                for (int j = minX; j < minX + 3; j++)
                {
                    if (this.numbers[i][j] == 0)
                    {
                        continue;
                    }

                    if (line.Contains(this.numbers[i][j]) == true)
                    {
                        return false;
                    }
                    else
                    {
                        line.Add(this.numbers[i][j]);
                    }
                }
            }

            return true;
        }

        /*--------------------------------------------------------------*/

        /*
        vrátí SortedSet èísel, které nejsou použity v daném øádku, sloupci a ètverci
         */
        private SortedSet<int> getUnusedNumbers(int y, int x)
        {
            SortedSet<int> used = getUsedNumbers(y, x);
            SortedSet<int> unused = new SortedSet<int>();
            
            for (int i = 1; i < 10; i++)
            {
                if (used.Contains(i) == false)
                {
                    unused.Add(i);
                }
            }

            return unused;
        }

        /*
        podle souøadnic v argumentech vrátí SortedSet èísel
        sloupce, øádku a ètverci 3x3 v herním poli
         */
        private SortedSet<int> getUsedNumbers(int y, int x)
        {
            var line = this.getLine(y);
            var row = this.getRow(x);
            var rect = this.getRect(y,x);

            SortedSet<int> set = new SortedSet<int>();
            set.UnionWith(line);
            set.UnionWith(row);
            set.UnionWith(rect);

            return set;
        }

        /*
        podle id v argumentu vrátí SortedSet èísel øádku v herním poli
         */
        private SortedSet<int> getLine(int id)
        {
            var line = new SortedSet<int>();
            if (!(id < 0 || id >= this.size))
            {
                for (int i = 0; i < this.size; i++)
                {
                    line.Add(this.numbers[id][i]);
                }    
            }

            return line;
        }

        /*
        podle id v argumentu vrátí SortedSet èísel sloupce v herním poli
         */
        private SortedSet<int> getRow(int id)
        {
            var line = new SortedSet<int>();
            if (!(id < 0 || id >= this.size))
            {
                for (int i = 0; i < this.size; i++)
                {
                    line.Add(this.numbers[i][id]);
                }
            }

            return line;
        }
        
        /*
        podle souøadnic v argumentech vrátí SortedSet dat ve ètverci 3x3 v herním poli
         */
        private SortedSet<int> getRect(int y, int x)
        {
            var line = new SortedSet<int>();
            if (!(x < 0 || y < 0 || x >= 9 || y >= 9))
            {
                int minY = y / 3;
                minY *= 3;
                for (int i = minY; i < minY + 3; i++)
                {
                    int minX = x / 3;
                    minX *= 3;
                    for (int j = minX; j < minX + 3; j++)
                    {
                        line.Add(this.numbers[i][j]);
                    }
                }
            }
            return line;
        }

        /*
        získá hodnotu z pole sudoku,
        podle pøedaných hodnot
         */
        public int get(int y, int x)
        {
            if (y < 0 || x < 0 || y >= size || x >= size)
            {
                return -1;
            }
            else
            {
                return numbers[y][x];
            }
        }

        /*
        nastaví hodnotu v poli sudoku,
        podle pøedaných hodnot
         */
        public void set(int y, int x, int val)
        {
            if (val < 0 || val > 9) return;

            if (!(y < 0 || x < 0 || y >= size || x >= size))
            {
                numbers[y][x] = val;
            }
        }

        /*
        Podle vstupních hodnot funkce urèí,
        jaký je název souboru se vstupními daty
         */
        public string getInputFilePath(TextBox textBox)
        {
            if (textBox.Text != string.Empty && textBox.Text != "")
            {
                return textBox.Text;
            }

            if (this.inputFilePath == "")
            {
                return @"input_text.txt";
            }

            return this.inputFilePath;
        }

        /*
        naète sudoku ze souboru
         */
        public bool loadFile(TextBox textBox)
        {
            string path = this.getInputFilePath(textBox);
            if (!System.IO.File.Exists(path))
            {
                return false;
            }

            Debug.WriteLine("Loading data...");

            string[] lines = System.IO.File.ReadAllLines(path);
            if (lines.Length != this.size)
            {
                return false;
            }

            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    if (lines[i][j] >= '0' && lines[i][j] <= '9')
                    {
                        this.numbers[i][j] = lines[i][j] - '0';
                    }
                }
            }

            return true;
        }

        /*
        uloží hodnoty sudoku do souboru
         */
        public void save()
        {
            Debug.WriteLine("Saving data...");

            var stringBuilder = new StringBuilder();
            for (int i = 0; i < this.numbers.Length; i++)
            {
                for (int j = 0; j < this.numbers[i].Length; j++)
                {
                    stringBuilder.Append(this.numbers[i][j].ToString());
                }
                stringBuilder.Append("\n");
            }

            File.WriteAllText(this.outputFilePath, stringBuilder.ToString());
        }

        /*
        testovací funkce
        vypíše do konzole sudoku v textové podobì
         */
        public void print()
        {
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    Debug.Write(this.numbers[i][j] + " ");
                }
                Debug.WriteLine("");
            }
        }

        /*
        testovací funkce
        vypíše do konzole sorted set
         */
        private void printLine(SortedSet<int> line)
        {
            Debug.Write("[ ");
            foreach (var item in line)
            {
                Debug.Write(item + " ");
            }
            Debug.WriteLine("]");
        }
                
        /*
        vynuluje pole numbers 
         */
        public void reset()
        {
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    this.set(i, j, 0);
                }
            }
        }
    }
}
