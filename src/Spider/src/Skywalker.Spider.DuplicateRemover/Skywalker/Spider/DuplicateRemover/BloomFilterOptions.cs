namespace Skywalker.Spider.DuplicateRemover;

public class BloomFilterOptions
{
    public double FalsePositiveProbability { get; set; }

    public int ExpectedInsertions { get; set; }
}
