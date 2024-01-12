using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SelectBD
{
    public partial class Form1 : Form
    {

        private string connectionString;
        private string ultimoComandoExecutado;

        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btnTestarConexao_Click(object sender, EventArgs e)
        {
            string servidor = txtServidor.Text;
            string bancoDados = txtBancoDados.Text;
            string usuario = txtUsuario.Text;
            string senha = txtSenha.Text;

            string connectionString = $"Data Source={servidor};Initial Catalog={bancoDados};User ID={usuario};Password={senha}";

            using (SqlConnection conexao = new SqlConnection(connectionString))
            {
                try
                {
                    conexao.Open();

                    // Conexão bem-sucedida, armazene os dados e exiba o nome do banco de dados
                    MessageBox.Show($"Conexão bem-sucedida com o banco de dados '{bancoDados}'", "Teste de Conexão", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    statusLabel.Text = $"Conectado ao banco de dados: {bancoDados}";
                }
                catch (Exception ex)
                {
                    // Erro ao conectar, exiba uma mensagem de erro
                    MessageBox.Show($"Erro ao conectar: {ex.Message}", "Erro de Conexão", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    statusLabel.Text = "Erro ao conectar";
                }

                finally
                {
                    // Garanta que a conexão seja sempre fechada, mesmo em caso de erro
                    if (conexao.State == System.Data.ConnectionState.Open)
                        conexao.Close();
                }


            }
        }

        private async void btnExeComando_Click(object sender, EventArgs e)
        {

            

            string servidor = txtServidor.Text;
            string bancoDados = txtBancoDados.Text;
            string usuario = txtUsuario.Text;
            string senha = txtSenha.Text;

            string connectionString = $"Data Source={servidor};Initial Catalog={bancoDados};User ID={usuario};Password={senha}";

            string comandoSql = txtComando.Text.Trim();

            using (SqlConnection conexao = new SqlConnection(connectionString))
            {
                try
                {
                    await conexao.OpenAsync();

                    using (SqlCommand comando = new SqlCommand(comandoSql, conexao))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(comando);
                        DataTable dataTable = new DataTable();

                        // Use o método FillAsync para permitir que a barra de progresso seja atualizada durante o preenchimento dos dados
                        await Task.Run(() => adapter.Fill(dataTable));

                        // Exibe os dados no DataGridView
                        ViewSelect.DataSource = dataTable;
                    }

                    ultimoComandoExecutado = comandoSql;
                    statusLabel.Text = "Comando executado com sucesso.";
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Erro ao executar o comando: {ex.Message}", "Erro de Comando", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    statusLabel.Text = "Erro ao executar o comando.";
                }
                finally
                {
                    if (conexao.State == ConnectionState.Open)
                        conexao.Close();

                    
                }
            }
        }

        
        
        private void button3_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ultimoComandoExecutado))
            {
                try
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Arquivo de Texto|*.txt";
                    saveFileDialog.Title = "Salvar Resultado em TXT";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string caminhoArquivo = saveFileDialog.FileName;

                        using (StreamWriter writer = new StreamWriter(caminhoArquivo))
                        {
                            // Escreve o comando executado no arquivo
                            writer.WriteLine("############################################");
                            writer.WriteLine($"Comando Executado: {ultimoComandoExecutado}");
                            writer.WriteLine("############################################");
                            writer.WriteLine();

                            // Escreve os cabeçalhos das colunas
                            foreach (DataGridViewColumn coluna in ViewSelect.Columns)
                            {
                                writer.Write($"{coluna.HeaderText}\t");
                            }
                            writer.WriteLine();

                            // Escreve os dados do DataGridView em formato tabular
                            foreach (DataGridViewRow linha in ViewSelect.Rows)
                            {
                                foreach (DataGridViewCell cell in linha.Cells)
                                {
                                    writer.Write($"{cell.Value}\t");
                                }
                                writer.WriteLine();
                            }

                            MessageBox.Show("Arquivo TXT gerado com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao gerar o arquivo TXT: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Não há dados para serem salvos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    
    }
}
