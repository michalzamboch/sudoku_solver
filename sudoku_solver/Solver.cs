using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace sudoku_solver
{
    /*
    T��da reprezentuje algorimus pro vyhodnocen� sudoku
    T��da obsahuje:
     - size - velikost sudoku = 9
     - numbers - hern� pole
     - inputFilePath - n�zev vstupn�ho souboru
     - outputFilePath - n�zev v�stupn�ho souboru
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
        funkce zkontroluje jestli jsou vstupn� data validn�,
        pokud ano, tak se zavol� algorimus pro �e�en� sudoku
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
        Algorimus pro �e�en� sudoku
        - backtracking

        1. nejpvve se hled� voln� m�sto v hrac�m poli,
           jakmile se toto m�sto najde, tak se ulo�� do prom�nn�ch
           row, column
        2. pokud se projde p�es cel� pole bez toho ani� by na�lo pr�zdn� m�sto,
           tak algoritmus vr�t� true a sudoku bylo vy�e�eno
        3. pokud sudoku nebylo doposud vy�e�eno, tak se pokra�uje d�le,
           kde se pro danou volnou pozici najdou v�echny ��sla, kter� jdu zapsat na danou pozici
           Tyto hodnoty jsou ulo�eny v SortedSet safeNumbers
        4. za�ne se iterovat p�es list safeNumbers.
           Vezme se prvn� ��slo z listu a vlo�� se na danou volnou pozici
        5. N�slden� se v podm�nce rekurzivn� zavol� funkce solveSudoku,
           kde se op�t hled� voln� m�sto a proces se tak opakuje
        6. pokud se ov�em nalezne voln� m�sto, kde nejde vlo�it ��dn� ��slo
           tak funkce vrac� false a pr�b�h algorimu se vr�t� o jedno zavol�n� funkce
           sudokuSolve a nastav� se, p�ede�lo pozici nula a zkus� se zde vlo�it nov� ��slo.
        7. tento proces prob�h� do t� doby do se na dan� m�sto nanastav� ta spr�vn� hodnota,
           kter� nebude nikde s ni��m kolidovat
        8. pokud se funkce vr�t� �pln� na za��tek procesu, to znamen�, �e v�e co funkce do
           hern�ho pole zapsala bylo tak� smaz�no a ji� nejsou dal�� hodnoty kter� by se daly vyzkou�et,
           algoritmus kon�� a vrac� false
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
        funkce kontroluje, jestli jsou vstupn� data pro algoritmus validn�
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
        funkce kontroluje jestli se v dan�m ��dku, v hern�m poli, nenach�z� stejn� ��sla
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
        funkce kontroluje jestli se v dan�m sloupci, v hern�m poli, nenach�z� stejn� ��sla
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
        funkce kontroluje jestli se v dan�m �tverci, v hern�m poli, nenach�z� stejn� ��sla
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
        vr�t� SortedSet ��sel, kter� nejsou pou�ity v dan�m ��dku, sloupci a �tverci
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
        podle sou�adnic v argumentech vr�t� SortedSet ��sel
        sloupce, ��dku a �tverci 3x3 v hern�m poli
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
        podle id v argumentu vr�t� SortedSet ��sel ��dku v hern�m poli
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
        podle id v argumentu vr�t� SortedSet ��sel sloupce v hern�m poli
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
        podle sou�adnic v argumentech vr�t� SortedSet dat ve �tverci 3x3 v hern�m poli
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
        z�sk� hodnotu z pole sudoku,
        podle p�edan�ch hodnot
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
        nastav� hodnotu v poli sudoku,
        podle p�edan�ch hodnot
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
        Podle vstupn�ch hodnot funkce ur��,
        jak� je n�zev souboru se vstupn�mi daty
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
        na�te sudoku ze souboru
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
        ulo�� hodnoty sudoku do souboru
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
        testovac� funkce
        vyp�e do konzole sudoku v textov� podob�
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
        testovac� funkce
        vyp�e do konzole sorted set
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
