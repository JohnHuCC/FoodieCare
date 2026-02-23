using FoodieCare.ModernApi.Domain;
using FoodieCare.ModernApi.Options;
using Microsoft.Extensions.Options;

namespace FoodieCare.ModernApi.Services;

public sealed class RuleBasedTypeRecommender
{
    private static readonly IReadOnlyDictionary<string, string> CategoryMap = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["Breakfast(good)"] = "早餐",
        ["LittleFood(good)"] = "小吃",
        ["JapnFood(good)"] = "日式料理",
        ["Barbecue(good)"] = "燒烤類",
        ["KoreaFood(good)"] = "韓式料理",
        ["Sweet(good)"] = "烘焙、甜點、零食",
        ["Afternoon_tea(good)"] = "下午茶",
        ["HotPot(good)"] = "鍋類",
        ["buffet(good)"] = "buffet自助餐",
        ["AsiaFood(good)"] = "亞洲料理"
    };

    private readonly RuleNode _root;

    public RuleBasedTypeRecommender(IOptions<FoodieCareOptions> options, IWebHostEnvironment env)
    {
        var rulePath = options.Value.RuleFile;
        if (!Path.IsPathRooted(rulePath))
        {
            rulePath = Path.Combine(env.ContentRootPath, rulePath);
        }

        _root = LoadTree(rulePath);
    }

    public string Recommend(PreferenceProfile profile)
    {
        if (string.Equals(profile.Taste, "sweety", StringComparison.Ordinal))
        {
            return string.Equals(profile.Hunger, "eat_less", StringComparison.Ordinal)
                ? "烘焙、甜點、零食"
                : "咖啡、簡餐、茶";
        }

        var features = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["price"] = profile.Price,
            ["eat_mode"] = profile.EatMode,
            ["hunger"] = profile.Hunger,
            ["distance"] = profile.Distance,
            ["hot_cold"] = profile.HotCold
        };

        var leaf = Evaluate(features);

        if (leaf == "烘焙、甜點、零食")
        {
            return string.Equals(profile.Hunger, "eat_less", StringComparison.Ordinal) ? "小吃" : "日式料理";
        }

        return leaf;
    }

    private string Evaluate(IReadOnlyDictionary<string, string> features)
    {
        var current = _root;
        string? best = null;

        while (current.Children.Count > 0)
        {
            RuleNode? next = null;
            RuleNode? fallback = null;

            foreach (var child in current.Children)
            {
                if (child.Value == "0")
                {
                    fallback = child;
                }

                if (!features.TryGetValue(child.Feature, out var inputValue))
                {
                    continue;
                }

                if (string.Equals(inputValue, child.Value, StringComparison.Ordinal))
                {
                    next = child;
                    break;
                }
            }

            next ??= fallback;
            if (next is null)
            {
                break;
            }

            if (!string.IsNullOrWhiteSpace(next.Prediction))
            {
                best = next.Prediction;
            }

            current = next;
        }

        return MapCategory(best ?? "LittleFood(good)");
    }

    private static string MapCategory(string raw)
    {
        var token = raw.Trim();
        return CategoryMap.TryGetValue(token, out var mapped) ? mapped : token;
    }

    private static RuleNode LoadTree(string rulePath)
    {
        if (!File.Exists(rulePath))
        {
            throw new FileNotFoundException($"Rule file not found: {rulePath}");
        }

        var root = new RuleNode { Feature = "root", Value = "root" };
        var stack = new List<RuleNode> { root };

        foreach (var rawLine in File.ReadLines(rulePath))
        {
            if (string.IsNullOrWhiteSpace(rawLine))
            {
                continue;
            }

            var (depth, content) = ParseIndentedLine(rawLine);
            var node = ParseRule(content);

            while (stack.Count > depth + 1)
            {
                stack.RemoveAt(stack.Count - 1);
            }

            stack[^1].Children.Add(node);
            stack.Add(node);
        }

        return root;
    }

    private static (int depth, string content) ParseIndentedLine(string rawLine)
    {
        var depth = 0;
        var i = 0;

        while (i < rawLine.Length)
        {
            if (rawLine[i] == '|')
            {
                depth++;
                i++;
                continue;
            }

            if (char.IsWhiteSpace(rawLine[i]))
            {
                i++;
                continue;
            }

            break;
        }

        return (depth, rawLine[i..].Trim());
    }

    private static RuleNode ParseRule(string content)
    {
        var eq = content.IndexOf('=');
        if (eq <= 0)
        {
            throw new FormatException($"Invalid rule row: {content}");
        }

        var feature = content[..eq].Trim();
        var right = content[(eq + 1)..].Trim();

        string value;
        string? prediction = null;

        var colon = right.IndexOf(':');
        if (colon >= 0)
        {
            value = right[..colon].Trim();
            var predictionRaw = right[(colon + 1)..].Trim();
            prediction = predictionRaw.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0].Trim();
        }
        else
        {
            value = right.Trim();
        }

        return new RuleNode
        {
            Feature = feature,
            Value = value,
            Prediction = prediction
        };
    }
}
