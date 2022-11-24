namespace TrafficCourts.Coms.Client.Test
{
    /// <summary>
    /// Tests annotated with this attribute requires you need to define INTEGRATION_TEST 
    /// in your build. Go to project properties > Build > Conditional Compilation Symbols.
    /// </summary>
    public class IntegrationTestTheoryAttribute : TheoryAttribute
    {
        public IntegrationTestTheoryAttribute()
        {
#if !INTEGRATION_TEST
            Skip = "Integration tests require additional configuration; useful for debug, but skipping here";
#endif
        }
    }
}
