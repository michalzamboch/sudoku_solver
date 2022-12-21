using System.Diagnostics;

/*
Grafické prostøedí - popis
 - tlaèítka
    - Load - naète soubor zadanı z textBoxu: Input File
    a vloí naètené data do herního pole,
    pokud nic zadáno nebylo, naète se soubor z pøedem definovaného zdroje v tøídì Solver
    - Solve - vyøeší zadané hodnoty
    - Save - data z herního pole jsou zapsána do souboru
    - Clear - vyèistí herní pole

 - herní pole 9x9
    - Mùe bıt naplnìno pomocí tlaèíka Load nebo hodnoty lze zadat manuálnì,
    staèí kliknout na buòku v poli a stisknout klávesu s poadovanım èíslem.
    Pokud je stisknuta 0, buòka se vynuluje.
*/

namespace sudoku_solver
{
    public partial class Form1 : Form
    {
        SudokuCell[,] cells = new SudokuCell[9, 9];
        string filePath = "input_text.txt";
        Solver solver;

        /*
        zde se inicializují poèáteèní data
         */
        public Form1()
        {
            InitializeComponent();
            
            this.solver = new Solver();
            this.solver.inputFilePath = this.filePath;

            createCells();
        }

        /*
        vytvoøí se dvourozmìrné herní pole,
        kadá buòka v poli je tvoøena pomocí tøídy SudokuSell
         - nastaví se vzhled bunìk, pozice a nulová hodnota
         - nakonec se kadé buòce pøiøadí funkce, která zajišuje kliknutí na buòku
         */
        private void createCells()
        {
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    cells[y, x] = new SudokuCell();
                    cells[y, x].Font = new Font(SystemFonts.DefaultFont.FontFamily, 20);
                    cells[y, x].Size = new Size(40, 40);
                    cells[y, x].ForeColor = SystemColors.ControlDarkDark;
                    cells[y, x].Location = new Point(x * 40, y * 40);
                    cells[y, x].FlatStyle = FlatStyle.Flat;
                    cells[y, x].FlatAppearance.BorderColor = Color.Black;
                    cells[y, x].X = x;
                    cells[y, x].Y = y;
                    cells[y, x].insert(0);

                    cells[y, x].KeyPress += cell_keyPressed;

                    panel1.Controls.Add(cells[y, x]);
                }
            }
        }

        /*
         - funkce zajišuje akci pøi kliknutí na danou buòku
         - pokud je stisknuta hodnota 0, tak se daná buòka vynuluje
         - pokud je stisknuta hodnota 1 a 9, tak se daná hodnota nahraje do buòky
         */
        private void cell_keyPressed(object sender, KeyPressEventArgs e)
        {
            var cell = sender as SudokuCell;

            if (cell.IsLocked)
                return;

            int value;

            if (int.TryParse(e.KeyChar.ToString(), out value))
            {
                if (value == 0)
                {
                    cell.clear();
                }
                else if (value >= 1 && value <= 9)
                {
                    this.solver.set(cell.Y, cell.X, value);
                    Debug.WriteLine(cell.Y + " : " + cell.X);
                    cell.insert(value);
                }
                cell.ForeColor = SystemColors.ControlDarkDark;
            }
        }

        /*
        fuknce vyèistí herní pole
         */
        private void clearPlayingField()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    this.cells[i, j].clear();
                }
            }
        }

        /*
        funkce zkontroluje jestli je herní pole prázdné
        to znamená, e nic není naètené
         */
        private bool playingFieldIsEmpty()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (this.cells[i, j].notZero() == true)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /*
        vloí hodnoty z tøídy Solver do vizuálního herního pole
         */
        private void insertAll()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var value = this.solver.get(i, j);
                    if (value == 0)
                    {
                        this.cells[i, j].clear();
                    }
                    else
                    {
                        this.cells[i, j].insert(value);
                    }
                }
            }
        }

        /*-------------------------- Button_Click -------------------------------*/

        /*
        tlaèítko pro naètení dat
        - vyèistí se herní pole a data ze tøídy Solver
        - naètou se data
        - o naètení èi nenaètení je uivatel informován
         */
        private void loadBtn_Click(object sender, EventArgs e)
        {
            this.clearPlayingField();
            this.solver.reset();

            Debug.WriteLine(this.filePathTextBox.Text);
            var loaded = this.solver.loadFile(this.filePathTextBox);

            if (loaded)
            {
                this.insertAll();
                this.resultLbl.Text = "Loaded from file: " + this.solver.getInputFilePath(this.filePathTextBox);
            }
            else
            {
                this.resultLbl.Text = "Input data can not be loaded.";
            }
        }

        /*
        tlaèítko pro vyøešení sudoku
        - zkontoluje se jestli herní pole není prázdné
        - vyøeší se sudoku
        - o vyslednu je uivatel informován
        - pokud se podaøilo sudoku vyøešit, tak se data vloí do herního pole
         */
        private void solveBtn_Click(object sender, EventArgs e)
        {
            if (this.playingFieldIsEmpty() == true)
            {
                this.resultLbl.Text = "Playing field is empty.";
                return;
            }

            this.solver.print();
            bool result = this.solver.solve();
            Debug.WriteLine("Solver result: " + result);
            if (result == true)
            {
                this.resultLbl.Text = "Successfully solved.";
                this.insertAll();
            }
            else
            {
                this.resultLbl.Text = "Sudoku can not be solved.";
            }
        }

        /*
        tlaèítko pro vyèištìní sudoku
        - vyèistí se herní pole
        - vyèistí se data z instance tøídy Solver
         */
        private void clearBtn_Click(object sender, EventArgs e)
        {
            this.clearPlayingField();
            this.solver.reset();
            this.resultLbl.Text = "Sudoku reseted.";
        }

        /*
        tlaèítko pro uloení vısledku sudoku
        - kontoluje se jestli herní pole není prázdné
         */
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (this.playingFieldIsEmpty() == false)
            {
                this.solver.save();
                this.resultLbl.Text = "Result saved to the file: " + this.solver.outputFilePath;
            }
            else
            {
                this.resultLbl.Text = "Playing field is empty.";
            }
        }

    }
}