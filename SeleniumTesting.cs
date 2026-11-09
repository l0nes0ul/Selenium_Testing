using NUnit;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

public class SeleniumTesting
{
    private IWebDriver driver;

    [SetUp]
    public void Setup()
    {
        driver = new EdgeDriver();
        driver.Manage().Window.Maximize();
    }

    [TearDown]
    public void Teardown()
    {
        driver.Quit();
        driver.Dispose();
    }
}