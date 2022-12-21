using System.Diagnostics;

/*
Grafick� prost�ed� - popis
 - tla��tka
    - Load - na�te soubor zadan� z textBoxu: Input File
    a vlo�� na�ten� data do hern�ho pole,
    pokud nic zad�no nebylo, na�te se soubor z p�edem definovan�ho zdroje v t��d� Solver
    - Solve - vy�e�� zadan� hodnoty
    - Save - data z hern�ho pole jsou zaps�na do souboru
    - Clear - vy�ist� hern� pole

 - hern� pole 9x9
    - M��e b�t napln�no pomoc� tla��ka Load nebo hodnoty lze zadat manu�ln�,
    sta�� kliknout na bu�ku v poli a stisknout kl�vesu s po�adovan�m ��slem.
    Pokud je stisknuta 0, bu�ka se vynuluje.
*/

namespace sudoku_solver
{
    public partial class Form1 : Form
    {
        SudokuCell[,] cells = new SudokuCell[9, 9];
        string filePath = "input_text.txt";
        Solver solver;

        /*
        zde se inicializuj� po��te�n� data
         */
        public Form1()
        {
            InitializeComponent();
            
            this.solver = new Solver();
            this.solver.inputFilePath = this.filePath;

            createCells();
        }

        /*
        vytvo�� se dvourozm�rn� hern� pole,
        ka�d� bu�ka v poli je tvo�ena pomoc� t��dy SudokuSell
         - nastav� se vzhled bun�k, pozice a nulov� hodnota
         - nakonec se ka�d� bu�ce p�i�ad� funkce, kter� zaji��uje kliknut� na bu�ku
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
         - funkce zaji��uje akci p�i kliknut� na danou bu�ku
         - pokud je stisknuta hodnota 0, tak se dan� bu�ka vynuluje
         - pokud je stisknuta hodnota 1 a� 9, tak se dan� hodnota nahraje do bu�ky
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
        fuknce vy�ist� hern� pole
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
        funkce zkontroluje jestli je hern� pole pr�zdn�
        to znamen�, �e nic nen� na�ten�
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
        vlo�� hodnoty z t��dy Solver do vizu�ln�ho hern�ho pole
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
        tla��tko pro na�ten� dat
        - vy�ist� se hern� pole a data ze t��dy Solver
        - na�tou se data
        - o na�ten� �i nena�ten� je u�ivatel informov�n
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
        tla��tko pro vy�e�en� sudoku
        - zkontoluje se jestli hern� pole nen� pr�zdn�
        - vy�e�� se sudoku
        - o vyslednu je u�ivatel informov�n
        - pokud se poda�ilo sudoku vy�e�it, tak se data vlo�� do hern�ho pole
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
        tla��tko pro vy�i�t�n� sudoku
        - vy�ist� se hern� pole
        - vy�ist� se data z instance t��dy Solver
         */
        private void clearBtn_Click(object sender, EventArgs e)
        {
            this.clearPlayingField();
            this.solver.reset();
            this.resultLbl.Text = "Sudoku reseted.";
        }

        /*
        tla��tko pro ulo�en� v�sledku sudoku
        - kontoluje se jestli hern� pole nen� pr�zdn�
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