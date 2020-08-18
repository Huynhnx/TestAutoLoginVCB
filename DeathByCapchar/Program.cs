using System;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using DeathByCaptchaSharp;
using System.Security.Policy;
using System.Net;
using java.awt.image;
using java.net;
using javax.imageio;
using System.Drawing;
using System.Threading;
using System.Collections.ObjectModel;
using java.sql;

namespace DeathByCapchar
{
    public static class Program
    {
        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)

        {

            if (timeoutInSeconds > 0)

            {

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));

                return wait.Until(drv => drv.FindElement(by));

            }

            return driver.FindElement(by);

        }
        static void Main(string[] args)
        {
            Client client = new Client("User", "Password");
            try
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArguments("disable-infobars");
                // Initialize the Chrome Driver
                using (var driver = new ChromeDriver(options))
                {
                    // Go to the home page
                    driver.Navigate().GoToUrl("https://vcbdigibank.vietcombank.com.vn/#/login?returnUrl=%2Fhome");

                    // Get the page elements
                    var userNameField = driver.FindElementById("username");
                    var userPasswordField = driver.FindElementByName("pass");

                    var capchar = driver.FindElementByName("captcha");
                    var img = driver.FindElementByXPath("/html/body/app-root/ng-component/div[1]/div/div[3]/div/div/div/div/div/div[4]/form[1]/div/div[3]/div/div[2]/div/img");
                    var value = img.GetAttribute("src");

                    WebClient clt = new WebClient();
                    Stream stream = clt.OpenRead(value);
                    Image image = Image.FromStream(stream);
                    byte[] bytearr = ImageToByteArray(image);
                    string res = string.Empty;
                    int logintime = 0;
                    bool isGetDone = false;
                    Task<Server> x = client.ServerStatus();
                    if (x.IsCompleted == false)
                    {
                        return;
                    }
                    while (logintime <4)
                    {
                        logintime++;
                        var captcha = client.Decode(bytearr, 30);
                        if (null != captcha && captcha.IsCompleted)
                        {
                            /* The CAPTCHA was solved; captcha.id property holds its numeric ID,
                               and captcha.text holds its text. */
                            //                       Console.WriteLine("CAPTCHA " + captcha.id + " solved: " + captcha.text);
                            
                            // Type user name and password
                            userNameField.SendKeys("0962809194");
                            userPasswordField.SendKeys("Son0972301962");
                            res = Console.ReadLine();

                            capchar.SendKeys(res);

                            // and click the login button
                            driver.FindElementById("btnLogin").Click();

                            var link = driver.FindElementByXPath("/html/body/app-root/ng-component/div/div[4]/div[2]/div[1]/div[1]/div/div[2]/a").GetAttribute("href");
                            driver.Navigate().GoToUrl(link);
                            var deb = driver.FindElementByXPath("/html/body/app-root/ng-component/div/div[5]/ng-component/div/div/div[2]/div/div/div[2]/div[1]/div[2]/p");
                            isGetDone = true;
                            Console.WriteLine(deb.Text);
                            break;
                        }
                    }
                    if (isGetDone == false)
                    {
                        Console.WriteLine("Account will be lock if you are login false one more time");
                    }
                    Console.ReadLine();
                }
            }
            catch (AccessDeniedException ex)
            {

            }
           
        }
    }
}
