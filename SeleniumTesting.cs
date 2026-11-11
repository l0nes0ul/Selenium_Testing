using NUnit;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

public class SeleniumTesting
{
    private IWebDriver driver;
    private const string BaseUrl = "https://the-internet.herokuapp.com/";

    [SetUp]
    public void Setup()
    {
        driver = new EdgeDriver();
        driver.Manage().Window.Maximize();
    }

    [Test]
    public void CheckboxesTest()
    {
        driver.Navigate().GoToUrl(BaseUrl + "checkboxes");
        
        var checkboxes = driver.FindElements(By.CssSelector("input[type='checkbox']"));
        
        Assert.That(checkboxes[0].Selected, Is.False);
        Assert.That(checkboxes[1].Selected, Is.True);
        
        foreach (var checkbox in checkboxes)
        {
            if (!checkbox.Selected)
            {
                checkbox.Click();
            }
        }

        Assert.That(checkboxes[0].Selected, Is.True);
        Assert.That(checkboxes[1].Selected, Is.True);
    }

    [Test]
    public void AddRemoveElementsTest()
    {
        driver.Navigate().GoToUrl(BaseUrl + "add_remove_elements/");
        
        var addButton = driver.FindElement(By.CssSelector("button[onclick='addElement()']"));
        addButton.Click();
        addButton.Click();
        
        var deleteButtons = driver.FindElements(By.ClassName("added-manually"));
        Assert.That(deleteButtons, Has.Count.EqualTo(2));
        
        deleteButtons[0].Click();
        
        deleteButtons = driver.FindElements(By.ClassName("added-manually"));
        Assert.That(deleteButtons, Has.Count.EqualTo(1));
    }

    [Test]
    public void DropdownTest()
    {
        driver.Navigate().GoToUrl(BaseUrl + "dropdown");
        
        var dropdownElement = driver.FindElement(By.Id("dropdown"));
        var select = new SelectElement(dropdownElement);
        
        select.SelectByValue("1");
        
        Assert.That(select.SelectedOption.Text, Is.EqualTo("Option 1"));

        select.SelectByText("Option 2");

        Assert.That(select.SelectedOption.Text, Is.EqualTo("Option 2"));
    }

    [Test]
    public void InputsTest()
    {
        driver.Navigate().GoToUrl(BaseUrl + "inputs");
        
        var inputField = driver.FindElement(By.CssSelector("input[type='number']"));
        inputField.SendKeys("14");
        Assert.That(inputField.GetAttribute("value"), Is.EqualTo("14"));

        inputField.SendKeys(Keys.ArrowUp);
        Assert.That(inputField.GetAttribute("value"), Is.EqualTo("15"));

        inputField.Clear();
        inputField.SendKeys("abc");
        Assert.That(inputField.GetAttribute("value"), Is.EqualTo(string.Empty));
    }

    [Test]
    public void StatusCodesTest()
    {
        driver.Navigate().GoToUrl(BaseUrl + "status_codes");
        
        driver.FindElement(By.LinkText("404")).Click();
        Assert.That(driver.Url, Does.EndWith("status_codes/404"));

        var contentText = driver.FindElement(By.CssSelector(".example p")).Text;
        Assert.That(contentText, Does.Contain("This page returned a 404 status code"));

        driver.FindElement(By.LinkText("here")).Click();
        Assert.That(driver.Url, Is.EqualTo(BaseUrl + "status_codes"));
    }

    [Test]
    public void NestedFramesTest()
    {
        driver.Navigate().GoToUrl(BaseUrl + "nested_frames");

        driver.SwitchTo().Frame("frame-top");
        var topFramesCount = driver.FindElements(By.TagName("frame")).Count;
        Assert.That(topFramesCount, Is.EqualTo(3));

        driver.SwitchTo().Frame("frame-left");
        var leftBodyText = driver.FindElement(By.TagName("body")).Text;
        Assert.That(leftBodyText, Does.Contain("LEFT"));

        driver.SwitchTo().ParentFrame();

        driver.SwitchTo().Frame("frame-middle");
        var middleBodyText = driver.FindElement(By.TagName("body")).Text;
        Assert.That(middleBodyText, Does.Contain("MIDDLE"));

        driver.SwitchTo().DefaultContent();

        driver.SwitchTo().Frame("frame-bottom");
        var bottomBodyText = driver.FindElement(By.TagName("body")).Text;
        Assert.That(bottomBodyText, Does.Contain("BOTTOM"));

        driver.SwitchTo().DefaultContent();
    }

    [Test]
    public void FormAuthenticationTest()
    {
        driver.Navigate().GoToUrl(BaseUrl + "login");

        var usernameInput = driver.FindElement(By.Id("username"));
        var passwordInput = driver.FindElement(By.Id("password"));
        var loginButton = driver.FindElement(By.CssSelector("button[type='submit']"));

        usernameInput.SendKeys("tomsmith");
        passwordInput.SendKeys("wrongPassword");
        loginButton.Click();

        var flashMessage = driver.FindElement(By.Id("flash"));
        Assert.That(flashMessage.Text, Does.Contain("Your password is invalid!"));

        usernameInput = driver.FindElement(By.Id("username"));
        passwordInput = driver.FindElement(By.Id("password"));
        loginButton = driver.FindElement(By.CssSelector("button[type='submit']"));
        
        usernameInput.SendKeys("tomsmith");
        passwordInput.SendKeys("SuperSecretPassword!");
        loginButton.Click();
        
        flashMessage = driver.FindElement(By.Id("flash"));
        Assert.That(flashMessage.Text, Does.Contain("You logged into a secure area!"));

        var logoutButton = driver.FindElement(By.CssSelector("a[href='/logout']"));
        logoutButton.Click();

        flashMessage = driver.FindElement(By.Id("flash"));
        Assert.That(flashMessage.Text, Does.Contain("You logged out of the secure area!"));
    }

    [Test]
    public void DynamicContentTest()
    {
        driver.Navigate().GoToUrl(BaseUrl + "dynamic_content");

        List<string> GetCurrentContentState()
        {
            var state = new List<string>();
            var images = driver.FindElements(By.CssSelector("#content .large-2 img"));
            var texts = driver.FindElements(By.CssSelector("#content .large-10"));

            for (int i = 0; i < images.Count; i++)
            {
                state.Add(images[i].GetAttribute("src"));
                state.Add(texts[i].Text);
            }
            return state;
        }

        var stateBefore = GetCurrentContentState();

        driver.Navigate().Refresh();

        var stateAfter = GetCurrentContentState();

        Assert.That(stateBefore, Is.Not.EqualTo(stateAfter));
    }

    [Test]
    public void DisappearingElementsTest()
    {
        driver.Navigate().GoToUrl(BaseUrl + "disappearing_elements");

        var menuButtons = driver.FindElements(By.CssSelector(".example ul li"));
        Assert.That(menuButtons, Has.Count.GreaterThanOrEqualTo(4));

        bool IsGalleryPresent()
        {
            var elements = driver.FindElements(By.LinkText("Gallery"));
            return elements.Count > 0;
        }

        bool initialState = IsGalleryPresent();
        bool stateChanged = false;
        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            driver.Navigate().Refresh();
            if (IsGalleryPresent() != initialState)
            {
                stateChanged = true;
                break;
            }
        }

        Assert.That(stateChanged, Is.True);
    }

    [Test]
    public void HoverTest()
    {
        driver.Navigate().GoToUrl(BaseUrl + "hovers");
        
        var avatars = driver.FindElements(By.ClassName("figure"));
        var caption1 = avatars[0].FindElement(By.ClassName("figcaption"));

        Assert.That(caption1.Displayed, Is.False);

        Actions action = new Actions(driver);
        action.MoveToElement(avatars[0]).Perform();
        
        Assert.That(caption1.Displayed, Is.True);
        Assert.That(caption1.Text, Does.Contain("name: user1"));

        action.MoveToElement(avatars[1]).Perform();
        var caption2 = avatars[1].FindElement(By.ClassName("figcaption"));
        var link2 = caption2.FindElement(By.TagName("a"));

        Assert.That(caption2.Displayed, Is.True);
        Assert.That(caption2.Text, Does.Contain("name: user2"));

        link2.Click();

        Assert.That(driver.Url, Does.Contain("/users/2"));

        var bodyText = driver.FindElement(By.TagName("body")).Text;

        Assert.That(bodyText, Does.Contain("Not Found"));
    }

    [TearDown]
    public void Teardown()
    {
        driver.Quit();
        driver.Dispose();
    }
}