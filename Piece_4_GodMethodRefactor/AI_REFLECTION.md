When I reviewed the code, I also felt that PricingService was the most difficult part to manage. There were too many if/else conditions for customer types and product categories, so even a small change could affect other logic. Claude suggestion of using the Strategy Pattern was actually very helpful. After separating each customer creates its own strategy class, the code became much cleaner and easier to maintain.

One thing I noticed was that there was no fallback strategy if a new customer type was added later. That could have caused a NullReferenceException during runtime. So I added StandardPricingStrategy as the default fallback to make the system safer.

Claude also suggested using a factory class,but for only three strategies it felt a bit too much. I kept it simple by using IEnumerable.FirstOrDefault, which made the code easier to read.

Copilot helped me the most while writing unit tests. For repetitive test cases, it saved a lot of time. I could just write a short comment and it generated most of the test code instantly.

But sometimes Copilot gives suggestions that look correct but are not ideal. For example, it suggested checking the exact exception message in tests. I avoided that because if the message changes later, the test will fail even though the functionality is correct. Instead, I only checked the exception type using Assert.ThrowsAsync<InvalidOperationException>.

If I am stuck debugging a production issue late at night, I would personally use Claude first. Copilot is great when I already know what I want to code. But Claude is more useful when I am confused, trying to understand what actually broke and need help tracing the problem through the whole flow.