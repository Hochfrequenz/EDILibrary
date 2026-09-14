using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using AwesomeAssertions;
using EDILibrary.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EDILibraryTests;

/// <summary>
/// Tests <see cref="InsertionOrderedDictionary"/>, the property bag
/// <see cref="EDILibrary.EdiJsonMapper"/> builds its JSON result from.
/// </summary>
/// <remarks>
/// This class exists for the sake of performance work, not to cover a functional requirement: the
/// type was introduced to replace <see cref="ExpandoObject"/>, which dominated the cost of parsing
/// segment-group-heavy messages. Each test therefore asserts the same thing of an ExpandoObject, so
/// that what is pinned down is "behaves like the type it replaced" rather than "behaves like its
/// own implementation". Only the behaviour the mapper relies on is covered - exception messages and
/// the Keys/Values collections deliberately differ and are not asserted against ExpandoObject.
/// </remarks>
[TestClass]
public class InsertionOrderedDictionaryTests
{
    private static IDictionary<string, object> Expando() => new ExpandoObject();

    private static IDictionary<string, object> Subject() => new InsertionOrderedDictionary();

    private static IEnumerable<IDictionary<string, object>> BothImplementations()
    {
        yield return Expando();
        yield return Subject();
    }

    [TestMethod]
    public void Enumerates_InInsertionOrder_NotSortedOrder()
    {
        foreach (var bag in BothImplementations())
        {
            foreach (string key in new[] { "zulu", "alpha", "mike", "bravo" })
            {
                bag.Add(key, key.ToUpperInvariant());
            }

            bag.Select(entry => entry.Key).Should().Equal("zulu", "alpha", "mike", "bravo");
        }
    }

    [TestMethod]
    public void Enumerates_InInsertionOrder_ForManyKeys()
    {
        var expected = Enumerable.Range(0, 500).Select(i => $"Feld{i}").ToList();

        foreach (var bag in BothImplementations())
        {
            foreach (string key in expected)
            {
                bag.Add(key, key);
            }

            bag.Select(entry => entry.Key).Should().Equal(expected);
        }
    }

    [TestMethod]
    public void Add_WithDuplicateKey_Throws()
    {
        foreach (var bag in BothImplementations())
        {
            bag.Add("Feld", "erster Wert");

            var add = () => bag.Add("Feld", "zweiter Wert");

            add.Should().Throw<System.ArgumentException>();
        }
    }

    [TestMethod]
    public void Indexer_OverwritingExistingKey_KeepsOriginalPosition()
    {
        foreach (var bag in BothImplementations())
        {
            bag.Add("erstes", 1);
            bag.Add("zweites", 2);
            bag.Add("drittes", 3);

            bag["erstes"] = 99;

            bag.Select(entry => entry.Key).Should().Equal("erstes", "zweites", "drittes");
            bag["erstes"].Should().Be(99);
        }
    }

    [TestMethod]
    public void Indexer_WithNewKey_AppendsAtTheEnd()
    {
        foreach (var bag in BothImplementations())
        {
            bag.Add("erstes", 1);

            bag["zweites"] = 2;

            bag.Select(entry => entry.Key).Should().Equal("erstes", "zweites");
        }
    }

    [TestMethod]
    public void Remove_KeepsTheOrderOfTheRemainingKeys()
    {
        foreach (var bag in BothImplementations())
        {
            foreach (string key in new[] { "a", "b", "c", "d" })
            {
                bag.Add(key, key);
            }

            bag.Remove("a").Should().BeTrue();
            bag.Remove("c").Should().BeTrue();
            bag.Remove("nicht vorhanden").Should().BeFalse();

            bag.Select(entry => entry.Key).Should().Equal("b", "d");
            bag["b"].Should().Be("b");
            bag["d"].Should().Be("d");
        }
    }

    /// <summary>
    /// Documents a deliberate divergence from <see cref="ExpandoObject"/>: after a key is removed
    /// and added again, ExpandoObject restores it at the slot it originally occupied, whereas this
    /// type appends it. <see cref="EDILibrary.EdiJsonMapper"/> never removes a key, so the mapper
    /// cannot observe the difference; this test exists so the divergence is recorded rather than
    /// discovered.
    /// </summary>
    [TestMethod]
    public void Remove_ThenReAdd_AppendsTheKey_UnlikeExpandoObject()
    {
        var subject = Subject();
        var expando = Expando();
        foreach (var bag in new[] { subject, expando })
        {
            foreach (string key in new[] { "a", "b", "c" })
            {
                bag.Add(key, key);
            }

            bag.Remove("a");
            bag.Add("a", "wieder da");
            bag["a"].Should().Be("wieder da");
        }

        subject.Select(entry => entry.Key).Should().Equal("b", "c", "a");
        expando.Select(entry => entry.Key).Should().Equal("a", "b", "c");
    }

    [TestMethod]
    public void ContainsKey_TryGetValue_AndCount_BehaveLikeADictionary()
    {
        foreach (var bag in BothImplementations())
        {
            bag.Add("da", "wert");

            bag.ContainsKey("da").Should().BeTrue();
            bag.ContainsKey("nicht da").Should().BeFalse();
            bag.TryGetValue("da", out object found).Should().BeTrue();
            found.Should().Be("wert");
            bag.TryGetValue("nicht da", out object missing).Should().BeFalse();
            missing.Should().BeNull();
            bag.Count.Should().Be(1);

            bag.Clear();
            bag.Count.Should().Be(0);
            bag.ContainsKey("da").Should().BeFalse();
        }
    }

    /// <summary>
    /// Documents a deliberate divergence from <see cref="ExpandoObject"/>: overwriting a value
    /// while enumerating throws on ExpandoObject but is tolerated here. This type is the more
    /// permissive of the two, so nothing that worked before can start failing.
    /// <see cref="EDILibrary.EdiJsonMapper"/> never mutates a bag it is enumerating.
    /// </summary>
    [TestMethod]
    public void OverwritingAValueWhileEnumerating_IsTolerated_UnlikeExpandoObject()
    {
        var subject = Subject();
        var expando = Expando();
        foreach (var bag in new[] { subject, expando })
        {
            bag.Add("a", 1);
            bag.Add("b", 2);
        }

        var overwriteWhileEnumerating = (IDictionary<string, object> bag) =>
            () =>
            {
                foreach (var entry in bag)
                {
                    bag[entry.Key] = 42;
                }
            };

        overwriteWhileEnumerating(subject).Should().NotThrow();
        overwriteWhileEnumerating(expando).Should().Throw<System.InvalidOperationException>();
        subject.Select(entry => entry.Value).Should().AllBeEquivalentTo(42);
    }
}
