using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

using Webbott.Support;
using Webbott.DOMSelectors;

namespace Webbott.Driver
{
    internal struct IFrameSelector
    {
        public By By { get; private set; }

        public int Index { get; private set; }

        public IFrameSelector(By by, int index)
        {
            By = by;
            Index = index;
        }
    }


    public class WebbottBrowserDriver
    {
        public string Url
        {
            get => seleniumDriver.Url;
            set
            {
                if(!value.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) 
                    && !value.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
                    value = $"http://{value}";

                seleniumDriver.Url = value;
            }
        }

        public string WindowTitle => seleniumDriver.Title;

        public bool IsActive { get; private set; }

        public string ThreadId { get; private set; }

        public Size WindowSize
        {
            get => seleniumDriver.Manage().Window.Size;
            private set
            {
                if (!isHeadless)
                    seleniumDriver.Manage().Window.Size = value;
            }
        }

        public Point WindowPosition
        {
            get => seleniumDriver.Manage().Window.Position;
            private set
            {
                if (!isHeadless)
                    seleniumDriver.Manage().Window.Position = value;
            }
        }

        public Actions ActionObject => new Actions(seleniumDriver);

        private IJavaScriptExecutor JavascriptExecutor => (IJavaScriptExecutor)seleniumDriver;

        private ITakesScreenshot ScreenshotMaker => (ITakesScreenshot)seleniumDriver;


        private bool isHeadless;

        private bool mouseButtonDown;

        private int maximumClickAttempts;

        private Random random;

        private IWebDriver seleniumDriver;


        public WebbottBrowserDriver() 
            => ApplyBrowserSettings(BrowserSettings.DefaultSettings);


        public WebbottBrowserDriver(BrowserSettings browserSettings)
            => ApplyBrowserSettings(browserSettings);


        public WebbottBrowserDriver(BrowserSettings browserSettings, string url)
        {
            ApplyBrowserSettings(browserSettings);
            Url = url;
        }


        private void ApplyBrowserSettings(BrowserSettings browserSettings)
        {
            if (browserSettings == null)
            {
                browserSettings = BrowserSettings.DefaultSettings;
            }
            else
            {
                browserSettings.MergeSettingsWith(BrowserSettings.DefaultSettings);
            }
            ThreadId = SharedConstants.DebugThreadId;
            isHeadless = browserSettings.HeadlessMode.Value;
            maximumClickAttempts = browserSettings.MaximumClickAttempts.Value;
            seleniumDriver = SeleniumWebDriverFactory.BuildDriver(browserSettings);
            IsActive = true;
            random = new Random(browserSettings.RandomSeed.Value);
        }


        // --------------------------
        // ACTIVE ELEMENT RETRIEVE METHODS
        // --------------------------

        private IWebElement GetActiveElement(WebbottSelectorBase selector)
            => GetActiveElement(selector, 0);


        private IWebElement GetActiveElement(WebbottSelectorBase selector, int index)
        {
            var by = selector.By();
            return GetActiveElement(by, index);
        }


        private IWebElement GetActiveElement(By selector, int index)
            => WaitForElementLoad(selector, index + 1, SharedConstants.DefaultTimeoutMilliseconds)[index];


        private List<IWebElement> GetActiveElements(WebbottSelectorBase selector)
            => GetActiveElements(selector, long.MaxValue);


        private List<IWebElement> GetActiveElements(WebbottSelectorBase selector, long maximumAmount)
        {
            var by = selector.By();
            return GetActiveElements(by, maximumAmount);
        }


        private List<IWebElement> GetActiveElements(By selector, long maximumAmount)
        {
            List<IWebElement> result = new List<IWebElement>();
            try
            {
                foreach(var domElement in seleniumDriver.FindElements(selector))
                {
                    try
                    {
                        if(domElement.IsActive())
                        {
                            result.Add(domElement);
                            if (result.Count > maximumAmount)
                                return result;
                        }
                    }
                    catch (StaleElementReferenceException) { }
                }
            }
            catch(WebDriverException) { }
            return result;
        }


        private IWebElement GetRandomActiveElement(WebbottSelectorBase selector)
        {
            List<IWebElement> list = GetActiveElements(selector);
            int randomIndex = random.Next(list.Count);
            return list[randomIndex];
        }


        // --------------------------
        // ELEMENT EXISTENCE METHODS
        // --------------------------

        public bool ElementExists(WebbottSelectorBase selector)
            => ElementExists(selector, 1);


        public bool ElementExists(WebbottSelectorBase selector, int desiredCount)
        {
            var elementList = GetActiveElements(selector);
            return elementList.Count >= desiredCount;
        }


        // --------------------------
        // ELEMENT LOAD/UNLOAD METHODS
        // --------------------------

        public List<IWebElement> WaitForElementLoad(WebbottSelectorBase selector)
            => WaitForElementLoad(selector, 1, SharedConstants.DefaultTimeoutMilliseconds);


        public List<IWebElement> WaitForElementLoad(WebbottSelectorBase selector, int minimumAmount)
            => WaitForElementLoad(selector, minimumAmount, SharedConstants.DefaultTimeoutMilliseconds);


        public List<IWebElement> WaitForElementLoad(WebbottSelectorBase selector, int minimumAmount, long timeoutMilliseconds)
        {
            var by = selector.By();
            return WaitForElementLoad(by, minimumAmount, timeoutMilliseconds);
        }


        private List<IWebElement> WaitForElementLoad(By selector, int minimumAmount, long timeoutMilliseconds)
        {
            DateTime endTime = DateTime.Now.AddMilliseconds(timeoutMilliseconds);
            List<IWebElement> result = GetActiveElements(selector, minimumAmount);
            while(result.Count < minimumAmount)
            {
                if (DateTime.Now > endTime)
                    throw WaitTimeoutException.ThrowNew();

                result = GetActiveElements(selector, minimumAmount);
            }
            return result;
        }


        public void WaitForElementUnload(WebbottSelectorBase selector)
            => WaitForElementUnload(selector, SharedConstants.DefaultTimeoutMilliseconds);


        public void WaitForElementUnload(WebbottSelectorBase selector, long timeoutMilliseconds)
        {
            DateTime endTime = DateTime.Now.AddMilliseconds(timeoutMilliseconds);
            while (ElementExists(selector))
                if (DateTime.Now > endTime)
                    throw WaitTimeoutException.ThrowNew();
        }


        // --------------------------
        // DOM ELEMENT METHODS
        // --------------------------

        public string GetText(WebbottSelectorBase selector)
            => GetText(selector, 0);

        
        public string GetText(WebbottSelectorBase selector, int index)
        {
            WaitForElementLoad(selector, index + 1);
            return GetActiveElement(selector, index).Text;
        }


        public void DeleteDOMElement(WebbottSelectorBase selector)
            => DeleteDOMElement(selector, 0);


        public void DeleteDOMElement(WebbottSelectorBase selector, int index)
        {
            string jsScript = selector.DeclareJavascriptVariable("domToDelete", index);
            jsScript += $" domToDelete.remove();";
            TryExecuteJavascript(jsScript);
        }


        // --------------------------
        // BROWSER WINDOW INPUT METHODS
        // --------------------------

        public void SetWindowSize(int width, int height)
            => WindowSize = new Size(width, height);


        public void SetWindowPosition(int x, int y)
            => WindowPosition = new Point(x, y);


        public void SetFullscreen()
            => seleniumDriver.Manage().Window.FullScreen();


        public void MaximizeWindow()
            => seleniumDriver.Manage().Window.Maximize();


        public void MinimizeWindow()
            => seleniumDriver.Manage().Window.Minimize();


        public void Refresh()
            => seleniumDriver.Navigate().Refresh();


        public void GoBack()
            => GoBack(1);


        public void GoBack(int amount)
        {
            for (int i = 0; i < amount; i++)
                seleniumDriver.Navigate().Back();
        }


        public void GoForward()
            => GoForward(1);


        public void GoForward(int amount)
        {
            for (int i = 0; i < amount; i++)
                seleniumDriver.Navigate().Forward();
        }

        #region Mouse Input Methods

        // CLICK METHODS

        public void Click()
        {
            Actions clickAction = ActionObject;
            clickAction.Click();
            clickAction.Build().Perform();
        }


        public void Click(WebbottSelectorBase selector)
            => Click(selector, 0);


        public void Click(WebbottSelectorBase selector, int index)
        {
            var by = selector.By();
            Click(by, index, 0);
        }


        private void Click(By selector, int index, int clickAttempt)
        {
            if (clickAttempt > maximumClickAttempts)
                throw ClickAttemptsExceededException.ThrowNew();

            WaitForElementLoad(selector, index + 1, SharedConstants.DefaultTimeoutMilliseconds);
            try
            {
                IWebElement target = GetActiveElement(selector, index);
                target.Click();
            }
            catch(WebDriverException e)
            {
                if(e.Message.Contains("is not clickable at point"))
                    ScrollDown(2);

                Click(selector, index, ++clickAttempt);
            }
        }

        // DOUBLE CLICK METHODS

        public void DoubleClick(WebbottSelectorBase selector)
            => DoubleClick(selector, 0);


        public void DoubleClick(WebbottSelectorBase selector, int index)
        {
            var by = selector.By();
            DoubleClick(by, index, 0);
        }


        private void DoubleClick(By selector, int index, int clickAttempt)
        {
            if (clickAttempt > maximumClickAttempts)
                throw ClickAttemptsExceededException.ThrowNew();

            Actions doubleClick = ActionObject;
            try
            {
                IWebElement target = GetActiveElement(selector, index);
                doubleClick.DoubleClick(target);
                doubleClick.Build().Perform();
            }
            catch (WebDriverException e)
            {
                if (e.Message.Contains("is not clickable at point"))
                    ScrollDown(2);

                Click(selector, index, ++clickAttempt);
            }
        }

        // RIGHT CLICK METHODS

        public void RightClick()
        {
            Actions rightClick = ActionObject;
            rightClick.ContextClick();
            rightClick.Build().Perform();
        }


        public void RightClick(WebbottSelectorBase selector)
            => RightClick(selector, 0);


        public void RightClick(WebbottSelectorBase selector, int index)
        {
            var by = selector.By();
            RightClick(by, index, 0);
        }


        private void RightClick(By selector, int index, int clickAttempt)
        {
            if (clickAttempt > maximumClickAttempts)
                throw ClickAttemptsExceededException.ThrowNew();

            Actions doubleClick = ActionObject;
            try
            {
                IWebElement target = GetActiveElement(selector, index);
                doubleClick.ContextClick(target);
                doubleClick.Build().Perform();
            }
            catch (WebDriverException e)
            {
                if (e.Message.Contains("is not clickable at point"))
                    ScrollDown(2);

                Click(selector, index, ++clickAttempt);
            }
        }

        // MOUSE BUTTON DOWN METHODS

        public void MouseButtonDown()
        {
            Actions mouseDown = ActionObject;
            mouseDown.ClickAndHold();
            mouseDown.Build().Perform();
            mouseButtonDown = true;
        }


        public void MouseButtonDown(WebbottSelectorBase selector)
            => MouseButtonDown(selector, 0);


        public void MouseButtonDown(WebbottSelectorBase selector, int index)
        {
            var by = selector.By();
            MouseButtonDown(by, index);
        }


        private void MouseButtonDown(By selector, int index)
        {
            IWebElement target = GetActiveElement(selector, index);
            Actions mouseDown = ActionObject;
            mouseDown.ClickAndHold(target);
            mouseDown.Build().Perform();
            mouseButtonDown = true;
        }

        // MOUSE BUTTON UP METHODS

        public void MouseButtonUp()
        {
            Actions mouseUp = ActionObject;
            mouseUp.Release();
            mouseUp.Build().Perform();
            mouseButtonDown = false;
        }


        public void MouseButtonUp(WebbottSelectorBase selector)
            => MouseButtonUp(selector, 0);


        public void MouseButtonUp(WebbottSelectorBase selector, int index)
        {
            var by = selector.By();
            MouseButtonUp(by, index);
            mouseButtonDown = false;
        }


        private void MouseButtonUp(By selector, int index)
        {
            IWebElement target = GetActiveElement(selector, index);
            Actions mouseUp = ActionObject;
            mouseUp.Release(target);
            mouseUp.Build().Perform();
        }


        // DRAG AND DROP METHODS

        public void DragAndDrop(WebbottSelectorBase fromSelector, WebbottSelectorBase toSelector)
            => DragAndDrop(fromSelector, 0, toSelector, 0);


        public void DragAndDrop(WebbottSelectorBase fromSelector, int fromIndex, WebbottSelectorBase toSelector, int toIndex)
        {
            var fromBy = fromSelector.By();
            var toBy = toSelector.By();
            DragAndDrop(fromBy, fromIndex, toBy, toIndex);
        }


        public void DragAndDrop(WebbottSelectorBase fromSelector, int offsetX, int offsetY)
            => DragAndDrop(fromSelector, 0, offsetX, offsetY);


        public void DragAndDrop(WebbottSelectorBase fromSelector, int selectorIndex, int offsetX, int offsetY)
        {
            var by = fromSelector.By();
            DragAndDrop(by, selectorIndex, offsetX, offsetY);
        }


        private void DragAndDrop(By fromSelector, int fromIndex, int offsetX, int offsetY)
        {
            IWebElement fromTarget = GetActiveElement(fromSelector, fromIndex);
            Actions dragAndDrop = ActionObject;
            dragAndDrop.DragAndDropToOffset(fromTarget, offsetX, offsetY);
            dragAndDrop.Build().Perform();
        }


        private void DragAndDrop(By fromSelector, int fromIndex, By toSelector, int toIndex)
        {
            IWebElement fromTarget = GetActiveElement(fromSelector, fromIndex);
            IWebElement toTarget = GetActiveElement(toSelector, toIndex);

            Actions clickAndDrag = ActionObject;
            clickAndDrag.ClickAndHold(fromTarget)
                .MoveToElement(toTarget)
                .Release(toTarget)
                .Build().Perform();
        }

        // MOUSE MOVEMENT METHODS

        public void MoveMouseTo(int x, int y) // Assuming origin is center of screen
        {
            bool mouseDown = mouseButtonDown;
            MouseButtonUp();
            CenterMouse();
            if (mouseDown)
                MouseButtonDown();
            
            var windowSize = WindowSize;
            if (Math.Abs(x) > windowSize.Width / 2)
                x = x.Sign() * windowSize.Width / 2;

            if (Math.Abs(y) > windowSize.Width / 2)
                y = y.Sign() * windowSize.Width / 2;

            MoveMouseOffset(x, y);
        }


        public void MoveMouseOffset(int offsetX, int offsetY)
        {
            Actions mouseMove = ActionObject;
            mouseMove.MoveByOffset(offsetX, offsetY);
            mouseMove.Build().Perform();
        }


        public void CenterMouse()
        {
            var bodyTag = new TagNameSelector("body");
            HoverOver(bodyTag);
        }


        public void HoverOver(WebbottSelectorBase selector)
            => HoverOver(selector, 0);


        public void HoverOver(WebbottSelectorBase selector, int index)
        {
            var by = selector.By();
            HoverOver(by, index);
        }


        private void HoverOver(By selector, int index)
        {
            WaitForElementLoad(selector, index + 1, SharedConstants.DefaultTimeoutMilliseconds);
            IWebElement target = GetActiveElement(selector, index);
            Actions mouseOver = ActionObject;
            mouseOver.MoveToElement(target);
            mouseOver.Build().Perform();
        }

        // SCROLL METHODS

        public void ScrollUp(int pixels)
            => Scroll(0, -pixels);


        public void ScrollDown(int pixels)
            => Scroll(0, pixels);


        public void ScrollLeft(int pixels)
            => Scroll(-pixels, 0);


        public void ScrollRight(int pixels)
            => Scroll(pixels, 0);


        public void Scroll(int rightPixels, int downPixels)
        {
            string javascript = $"window.scrollBy({rightPixels},{downPixels});";
            ExecuteJavascript(javascript);
        }

        #endregion


        // --------------------------
        // KEYBOARD INPUT METHODS
        // --------------------------

        public void Clear(WebbottSelectorBase selector)
            => Clear(selector, 0);


        public void Clear(WebbottSelectorBase selector, int index)
        {
            var by = selector.By();
            Clear(by, index);
        }


        private void Clear(By selector, int index)
        {
            IWebElement target = GetActiveElement(selector, index);
            target.Clear();
        }


        public void Type(string message)
        {
            Actions typeAction = ActionObject;
            typeAction.SendKeys(message);
            typeAction.Build().Perform();
        }


        public void Type(string message, WebbottSelectorBase selector)
            => Type(message, selector, 0, false);


        public void Type(string message, WebbottSelectorBase selector, bool pressEnterOnComplete)
            => Type(message, selector, 0, pressEnterOnComplete);


        public void Type(string message, WebbottSelectorBase selector, int index, bool pressEnterOnComplete)
        {
            var by = selector.By();
            Type(message, by, index, pressEnterOnComplete);
        }


        private void Type(string message, By selector, int index, bool pressEnterOnComplete)
        {
            WaitForElementLoad(selector, index + 1, SharedConstants.DefaultTimeoutMilliseconds);
            if (pressEnterOnComplete)
                message += SpecialKeys.Enter;

            IWebElement target = GetActiveElement(selector, index);
            target.SendKeys(message);
        }


        public void HoldKey(string key)
        {
            Actions keyDown = ActionObject;
            keyDown.KeyDown(key);
            keyDown.Build().Perform();
        }


        public void HoldKey(string key, WebbottSelectorBase selector)
            => HoldKey(key, selector, 0);


        public void HoldKey(string key, WebbottSelectorBase selector, int index)
        {
            var by = selector.By();
            HoldKey(key, by, index);
        }


        private void HoldKey(string key, By selector, int index)
        {
            IWebElement target = GetActiveElement(selector, index);
            Actions keyDown = ActionObject;
            keyDown.KeyDown(target, key);
            keyDown.Build().Perform();
        }


        public void ReleaseKey(string key)
        {
            Actions keyUp = ActionObject;
            keyUp.KeyUp(key);
            keyUp.Build().Perform();
        }


        public void ReleaseKey(string key, WebbottSelectorBase selector)
            => ReleaseKey(key, selector, 0);


        public void ReleaseKey(string key, WebbottSelectorBase selector, int index)
        {
            var by = selector.By();
            ReleaseKey(key, by, index);
        }


        private void ReleaseKey(string key, By selector, int index)
        {
            IWebElement target = GetActiveElement(selector, index);
            Actions keyDown = ActionObject;
            keyDown.KeyUp(target, key);
            keyDown.Build().Perform();
        }


        // --------------------------
        // IFRAME NAVIGATION METHODS
        // --------------------------

        public void FocusOnIframe(WebbottSelectorBase selector)
            => FocusOnIframe(selector, 0);


        public void FocusOnIframe(WebbottSelectorBase selector, int index)
        {
            var by = selector.By();
            FocusOnIframe(by, index);
        }


        private void FocusOnIframe(By selector, int index)
        {
            IWebElement target = GetActiveElement(selector, index);
            seleniumDriver.SwitchTo().Frame(target);
        }


        public void FocusOnDefaultContent()
            => seleniumDriver.SwitchTo().DefaultContent();


        public void FocusOnParentIframe()
            => seleniumDriver.SwitchTo().ParentFrame();


        // --------------------------
        // WINDOW NAVIGATION METHODS
        // --------------------------

        public void FocusOnWindow(int index)
        {
            string windowName = seleniumDriver.WindowHandles[index];
            FocusOnWindowName(windowName);
        }


        public void FocusOnWindowName(string name)
            => seleniumDriver.SwitchTo().Window(name);


        public void FocusOnNewestWindow()
        {
            string windowName = seleniumDriver.WindowHandles[^1];
            FocusOnWindowName(windowName);
        }


        public void FocusOnFirstWindow()
            => FocusOnWindow(0);


        public void FocusOnAlert()
            => seleniumDriver.SwitchTo().Alert();

        
        public void CloseWindow()
        {
            seleniumDriver.Close();
            FocusOnFirstWindow();
        }


        // --------------------------
        // COOKIE MANAGEMENT METHODS
        // --------------------------

        public void AddCookie(WebbottBrowserCookie cookie)
            => AddCookie(cookie, false);


        public void AddCookie(WebbottBrowserCookie cookie, bool overwrite)
        {
            Cookie seleniumCookie = cookie.ToSeleniumCookie();
            AddCookie(seleniumCookie, overwrite);
        }


        private void AddCookie(Cookie cookie, bool overwrite)
        {
            string cookieName = cookie.Name;
            var currentCookie = GetCookie(cookieName);
            if (currentCookie != null)
            {
                if (overwrite)
                    DeleteCookie(currentCookie.Name);
            }
            else
            {
                seleniumDriver.Manage().Cookies.AddCookie(cookie);
            }
        }


        public WebbottBrowserCookie GetCookie(string cookieName)
        {
            Cookie cookie = seleniumDriver.Manage().Cookies.GetCookieNamed(cookieName);
            if (cookie == null)
                return null;

            return WebbottBrowserCookie.FromSeleniumCookie(cookie);
        }


        public void DeleteCookie(string cookieName)
            => seleniumDriver.Manage().Cookies.DeleteCookieNamed(cookieName);


        public void DeleteCookie(WebbottBrowserCookie cookie)
        {
            var cookies = seleniumDriver.Manage().Cookies.AllCookies;
            var seleniumCookie = cookie.ToSeleniumCookie();
            seleniumDriver.Manage().Cookies.DeleteCookie(seleniumCookie);
        }


        public void ExpireCookies(DateTime expirationDate)
        {
            var cookieList = seleniumDriver.Manage().Cookies.AllCookies;
            foreach (var cookie in cookieList)
                if (cookie.Expiry.HasValue && cookie.Expiry.Value <= expirationDate)
                    DeleteCookie(cookie.Name);
        }


        public void ExpireCookies(Func<WebbottBrowserCookie, bool> evalFunction)
        {
            var cookieList = seleniumDriver.Manage().Cookies.AllCookies;
            WebbottBrowserCookie browserCookie;
            foreach(var cookie in cookieList)
            {
                browserCookie = WebbottBrowserCookie.FromSeleniumCookie(cookie);
                if (evalFunction(browserCookie))
                    seleniumDriver.Manage().Cookies.DeleteCookie(cookie);
            }
        }


        public void DeleteCookiesWithNoExpiration()
        {
            var cookieList = seleniumDriver.Manage().Cookies.AllCookies;
            foreach (var cookie in cookieList)
                if (!cookie.Expiry.HasValue)
                    DeleteCookie(cookie.Name);
        }


        public void ClearCookies()
            => seleniumDriver.Manage().Cookies.DeleteAllCookies();


        // --------------------------
        // JAVASCRIPT METHODS
        // --------------------------

        public void ExecuteJavascript(string jsScript)
        {
            IJavaScriptExecutor jsExecutor = JavascriptExecutor;
            jsExecutor.ExecuteScript(jsScript);
        }


        public void TryExecuteJavascript(string jsScript)
        {
            try { ExecuteJavascript(jsScript); }
            catch { }
        }


        public void ExecuteAsyncJavascript(string jsScript)
        {
            IJavaScriptExecutor jsExecutor = JavascriptExecutor;
            jsExecutor.ExecuteAsyncScript(jsScript);
        }


        public void TryExecuteAsyncJavascript(string jsScript)
        {
            try { ExecuteAsyncJavascript(jsScript); }
            catch { }
        }


        // --------------------------
        // WAIT METHODS
        // --------------------------

        public void Wait(TimeUnit timeUnit, int amount)
        {
            TimeSpan timeSpan = new TimeSpan();
            switch(timeUnit)
            {
                case TimeUnit.Milliseconds:
                    timeSpan = TimeSpan.FromMilliseconds(amount);
                    break;

                case TimeUnit.Seconds:
                    timeSpan = TimeSpan.FromSeconds(amount);
                    break;

                case TimeUnit.Minutes:
                    timeSpan = TimeSpan.FromMinutes(amount);
                    break;

                case TimeUnit.Hours:
                    timeSpan = TimeSpan.FromHours(amount);
                    break;

                case TimeUnit.Days:
                    timeSpan = TimeSpan.FromDays(amount);
                    break;

                case TimeUnit.Weeks:
                    timeSpan = TimeSpan.FromDays(7 * amount);
                    break;
            }
            Wait(timeSpan);
        }


        public void Wait(TimeSpan timeSpan)
            => Thread.Sleep(timeSpan);


        // --------------------------
        // MISCELLANEOUS METHODS
        // --------------------------

        public Uri WaitForUrl(string url)
            => WaitForUrl(url, SharedConstants.DefaultTimeoutMilliseconds);


        public Uri WaitForUrl(string url, int timeoutMilliseconds)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            DateTime endTime = DateTime.Now.AddMilliseconds(timeoutMilliseconds);
            string currentUrl = Url;

            if (currentUrl.Equals(url, StringComparison.InvariantCultureIgnoreCase))
                return new Uri(currentUrl);

            while (!currentUrl.Contains(url, StringComparison.InvariantCultureIgnoreCase))
            {
                if (DateTime.Now > endTime)
                    throw WaitTimeoutException.ThrowNew();

                try { currentUrl = Url; }
                catch { }
            }
            return new Uri(currentUrl);
        }


        public Uri WaitForUrlSubstring(string urlSubstring)
            => WaitForUrlSubstring(urlSubstring, SharedConstants.DefaultTimeoutMilliseconds);


        public Uri WaitForUrlSubstring(string urlSubstring, int timeoutMilliseconds)
        {
            if (string.IsNullOrEmpty(urlSubstring))
                throw new ArgumentNullException(nameof(urlSubstring));

            DateTime endTime = DateTime.Now.AddMilliseconds(timeoutMilliseconds);
            string currentUrl = Url;

            if (currentUrl.Contains(urlSubstring, StringComparison.InvariantCultureIgnoreCase))
                return new Uri(currentUrl);

            while(!currentUrl.Contains(urlSubstring, StringComparison.InvariantCultureIgnoreCase))
            {
                if (DateTime.Now > endTime)
                    throw WaitTimeoutException.ThrowNew();

                try { currentUrl = Url; }
                catch { }
            }
            return new Uri(currentUrl);
        }

        
        public void TakeScreenshot()
        {
            string fileName = $"{ThreadId}_{SupportMethods.CurrentTimeAsString}.png";
            TakeScreenshot(fileName);
        }


        public void TakeScreenshot(string fileName)
        {
            if (isHeadless)
                return;

            Screenshot screenshot = ScreenshotMaker.GetScreenshot();
            ScreenshotImageFormat imageFormat = ScreenshotImageFormat.Png;

            string formatString = fileName.Split('.')[^1];
            foreach (ScreenshotImageFormat format in Enum.GetValues(typeof(ScreenshotImageFormat)))
                if (format.ToString().Equals(formatString, StringComparison.InvariantCultureIgnoreCase))
                {
                    imageFormat = format;
                    break;
                }

            string outputPath = WebbottConfig.ScreenshotDirectory;
            outputPath += fileName;
            screenshot.SaveAsFile(outputPath, imageFormat);
        }


        public void Quit()
        {
            seleniumDriver.Quit();
            IsActive = false;
        }
    }
}
