namespace MindBlown.Types
{
public struct AnsweringStatsStruct {
      public int correctAnswerCount { get; set; }
      public int allAnswerCount { get; set; }
      public int precision { get; set; }
      public AnsweringStatsStruct() : this(2) {}
      public AnsweringStatsStruct(int setPrecision = 2)
      {
          precision = setPrecision;
      }
  }
}