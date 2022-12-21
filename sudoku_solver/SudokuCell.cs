using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sudoku_solver
{
    /*
    Třída reprezentuje sudouku buňku ve herním poli
    Třída obsahuje:
     - Value - hodnota v buňce
     - IsLocked - možnost editovat buňku
     - X - pozice x
     - Y - pozice y
    */
    class SudokuCell : Button
    {
        public int Value { get; set; }
        public bool IsLocked { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        // vyčistí buňku
        public void clear()
        {
            this.Text = string.Empty;
            this.IsLocked = false;
            this.Value = 0;
        }

        // vložení dat do buňky
        public void insert(int value)
        {
            this.Value = value;
            if (value == 0)
            {
                this.Text = string.Empty;
            }
            else
            {
                this.Text = value.ToString();
            }
        }

        // kontola, jestli hodnota v buňce není nulová
        public bool notZero()
        {
            return this.Value != 0;
        }
    }
}
