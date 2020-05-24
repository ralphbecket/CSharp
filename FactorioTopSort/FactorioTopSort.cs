using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FactorioTopSort
{
    public static class FactorioTopSort
    {
        // const string DefaultRecipesPath = @"C:\Program Files (x86)\GOG Galaxy\Games\Factorio\data\base\prototypes\recipe";
        // const string DefaultRecipesPath = @"E:\GOG Downloads\Factorio\data\base\prototypes\recipe";
        const string DefaultRecipesPath = @"C:\Program Files\Factorio\data\base\prototypes\recipe";

        internal class Recipe
        {
            public string Name;
            public List<(string, int)> Ingredients = new List<(string, int)> { };
        }

        internal static List<Recipe> ParseLuaRecipes(IEnumerable<string> paths)
        {
            return paths.SelectMany(path => ParseLuaRecipes(path)).ToList();
        }

        internal static List<Recipe> ParseLuaRecipes(string path)
        {
            var text = File.ReadAllText(path);
            var n = text.Length;
            var i = 0;
            var recipes = new List<Recipe> { };
            void seekToKeyword(string s)
            {
                var j = text.IndexOf(s, i);
                if (j == -1) throw new ApplicationException($"Can't find '{s}' past [{i}].");
                i = j + s.Length;
            }
            string seekToEither(string s, string t)
            {
                var j = text.IndexOf(s, i);
                var k = text.IndexOf(t, i);
                if (j == -1 && k != -1) { i = k; return t; }
                if (k == -1 && j != -1) { i = j; return s; }
                if (j == -1) throw new ApplicationException($"Can't find '{s}' or '{t}' past [{i}].");
                if (j < k)
                {
                    i = j; return s;
                }
                else
                {
                    i = k; return t;
                }
            }
            string seekStr()
            {
                var j = text.IndexOf('\"', i);
                if (j == -1) throw new ApplicationException($"Can't find a string past [{i}].");
                var k = text.IndexOf('\"', j + 1);
                if (j == -1) throw new ApplicationException($"Unterminated string at [{j}].");
                i = k + 1;
                return text.Substring(j + 1, k - j - 1);
            }
            int seekInt()
            {
                var j = i;
                while (!char.IsDigit(text[j]))
                {
                    if (n <= ++j) throw new ApplicationException($"Can't find int past [{i}].");
                }
                var k = text[j++] - '0';
                while (j < n && char.IsDigit(text[j])) k = (10 * k) + (text[j++] - '0');
                i = j;
                return k;
            }
            while (true)
            {
                try
                {
                    seekToKeyword("type = \"recipe\"");
                    seekToKeyword("name =");
                    var name = seekStr();
                    var recipe = new Recipe { Name = name };
                    seekToKeyword("ingredients =");
                    seekToKeyword("{");
                    while (true)
                    {
                        if (seekToEither("}", "{") == "}") break;
                        var item = seekStr();
                        if (item == "item" || item == "fluid") item = seekStr();
                        var quantity = seekInt();
                        seekToKeyword("}");
                        recipe.Ingredients.Add((item, quantity));
                    }
                    recipes.Add(recipe);
                }
                catch
                {
                    // We're done.
                    break;
                }
            }
            return recipes;
        }

        internal static Dictionary<Recipe, int> TopSortRecipes(IEnumerable<Recipe> recipes)
        {
            var index = new Dictionary<string, Recipe> { };
            // There are duplicates...
            foreach (var recipe in recipes) index[recipe.Name] = recipe;
            foreach (var ingredient in recipes.SelectMany(x => x.Ingredients))
            {
                var name = ingredient.Item1;
                if (index.ContainsKey(name)) continue;
                index[name] = new Recipe { Name = name };
            }
            var deps = recipes.ToDictionary(x => x, x => x.Ingredients.Select(i => index[i.Item1]));
            var ranks = new Dictionary<Recipe, int> { };
            int rankRecipe(Recipe r)
            {
                if (ranks.ContainsKey(r)) return ranks[r];
                var ingredients = r.Ingredients;
                var rank = (!ingredients.Any() ? 0 : 1 + ingredients.Max(i => rankRecipe(index[i.Item1])));
                ranks[r] = rank;
                return rank;
            }
            foreach (var recipe in recipes) rankRecipe(recipe);
            return ranks;
        }

        public static void Main(string[] args)
        {
            var paths = Directory.EnumerateFiles(DefaultRecipesPath, "*.lua");
            // paths = paths.Where(x => !Path.GetFileName(x).StartsWith("demo-"));
            var recipes = ParseLuaRecipes(paths);
            var index = new Dictionary<string, Recipe> { };
            // There are duplicates...
            foreach (var recipe in recipes) index[recipe.Name] = recipe;
            // var index = recipes.ToDictionary(x => x.Name, x => x);
            var ranks = TopSortRecipes(recipes);
            var usedBy = recipes.ToDictionary(x => x, x => recipes.Where(y => y.Ingredients.Any(z => z.Item1 == x.Name)).ToList());
            var needs = new List<string> { };
            var uses = new List<string> { };
            foreach (var kv in ranks.OrderBy(kv => kv.Value))
            {
                var recipe = kv.Key;
                var rank = kv.Value;
                var name = recipe.Name;
                var ingredients = string.Join(", ", recipe.Ingredients);
                var usedIn = (!usedBy.ContainsKey(recipe) ? "" : string.Join(", ", usedBy[recipe].Select(x => (x.Name, ranks[x])).OrderBy(x => x.Item2)));
                if (ingredients != "") needs.Add($"{rank,-3} {name,-30} {ingredients}");
                if (usedIn != "") uses.Add($"{rank,-3} {name,-30} {usedIn}");
            }
            Console.WriteLine("\n==== DEPENDENCIES ====\n");
            foreach (var line in needs) Console.WriteLine(line);
            Console.WriteLine("\n==== DEPENDENTS ====\n");
            foreach (var line in uses) Console.WriteLine(line);
        }
    }
}
