using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace RPAProject
{
    public class AutomationWeb
    {
        private IWebDriver driver;

        public AutomationWeb()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--disable-notifications"); // Desativa notificações
            options.AddArguments("--disable-popup-blocking"); // Desativa bloqueio de pop-ups

            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
        }

        public void TesteWeb()
        {
            // Acessar site
            driver.Navigate().GoToUrl("https://10fastfingers.com/typing-test/portuguese");

            // Aceitar cookies
            driver.FindElement(By.Id("CybotCookiebotDialogBodyButtonDecline")).Click();

            // Espera até que o campo de input esteja visível
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IWebElement inputField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("inputfield")));

            // Selecionar sentença
            IWebElement rowElement = driver.FindElement(By.Id("row1"));

            // Capturar todos os elementos 'span' dentro de 'rowElement'
            IReadOnlyCollection<IWebElement> spanElements = rowElement.FindElements(By.TagName("span"));

            foreach (IWebElement spanElement in spanElements)
            {
                string text = spanElement.Text.Trim();

                // Inserindo no campo de input
                inputField.SendKeys(text + " ");

                // Aguardar 0,1 segundo
                System.Threading.Thread.Sleep(100);
            }

            // Capturar os resultados diretamente
            string wpmElementText = driver.FindElement(By.Id("wpm")).Text;
            string wpm = Regex.Match(wpmElementText, @"\d+").Value;

            string keystrokesElementText = driver.FindElement(By.Id("keystrokes")).Text;
            string keystrokes = Regex.Match(keystrokesElementText, @"\d+").Value;

            string accuracyElementText = driver.FindElement(By.Id("accuracy")).Text;
            string accuracy = Regex.Match(accuracyElementText, @"\d+%").Value; // Regex exclusivo para porcentagem

            string correctWordsElementText = driver.FindElement(By.Id("correct")).Text;
            string correctWords = Regex.Match(correctWordsElementText, @"\d+").Value;

            string wrongWordsElementText = driver.FindElement(By.Id("wrong")).Text;
            string wrongWords = Regex.Match(wrongWordsElementText, @"\d+").Value;

            // Chama o método ConnectBD com os valores capturados
            ConnectBD(wpm, keystrokes, accuracy, correctWords, wrongWords);
        }

        public void ConnectBD(string wpm, string keystrokes, string accuracy, string correctWords, string wrongWords)
        {
            string connectionString = "Server=.\\sqlexpress;Database=projetoRPA;Trusted_Connection=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Chamada do método para inserir dados
                InserirDados(connection, wpm, keystrokes, accuracy, correctWords, wrongWords);
            }
        }

        static void InserirDados(SqlConnection connection, string wpm, string keystrokes, string accuracy, string correctWords, string wrongWords)
        {
            string query = "INSERT INTO resultadoRPA (wpm, keystrokes, accuracy, correctWords, wrongWords) VALUES (@wpm, @keystrokes, @accuracy, @correctWords, @wrongWords)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Adiciona parâmetros para evitar SQL injection
                command.Parameters.AddWithValue("@wpm", wpm);
                command.Parameters.AddWithValue("@keystrokes", keystrokes);
                command.Parameters.AddWithValue("@accuracy", accuracy);
                command.Parameters.AddWithValue("@correctWords", correctWords);
                command.Parameters.AddWithValue("@wrongWords", wrongWords);

                connection.Open();
                command.ExecuteNonQuery();
            }

            Console.WriteLine("Dados inseridos com sucesso!");
        }
    }
}
