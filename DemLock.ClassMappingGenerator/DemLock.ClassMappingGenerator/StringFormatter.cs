/// <summary>
/// Super simple string template engine for making reuse of string way easier.
///
/// In the future if I get more motivation to dump effort into speeding things up I will come back and
/// turn this into a compiled templating engine, but aggregate replace is goo enough for me!
/// </summary>
public class StringTemplate
{
    public string Template { get; set; }

    public Dictionary<string, object> Parameters { get; set; }

    public StringTemplate(string template)
    {
        Template = template;
        Parameters = new Dictionary<string, object>();
    }

    public string Execute(Dictionary<string, object> vars)
    {
        return vars.Aggregate(Template, (current, parameter) => current.Replace($"@{{{parameter.Key}}}", parameter.Value.ToString()));
    }
    public string Execute() => Execute(new());
}