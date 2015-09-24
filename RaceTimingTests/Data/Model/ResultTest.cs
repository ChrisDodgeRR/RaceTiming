using RedRat.RaceTiming.Data.Model;
using Xunit;

namespace RaceTimingTests.Data.Model
{
    public class ResultTest
    {
        [Fact]
        public void CanAddAndRemoveDubiousReason()
        {
            var result = new Result();
            Assert.Equal( Result.DubiousResultEnum.None, result.DubiousResult );

            // -- Adding --
            Result.AddDubiousReason( result, Result.DubiousResultEnum.DuplicateNumber );
            Assert.Equal( Result.DubiousResultEnum.DuplicateNumber, result.DubiousResult );

            Result.AddDubiousReason(result, Result.DubiousResultEnum.EstimatedTime);
            Assert.Equal(Result.DubiousResultEnum.DuplicateNumber | Result.DubiousResultEnum.EstimatedTime, result.DubiousResult);

            Result.AddDubiousReason(result, Result.DubiousResultEnum.EstimatedTime);
            Assert.Equal(Result.DubiousResultEnum.DuplicateNumber | Result.DubiousResultEnum.EstimatedTime, result.DubiousResult);

            // -- Removing --
            Result.RemoveDubiousReason( result, Result.DubiousResultEnum.DuplicateNumber );
            Assert.Equal( Result.DubiousResultEnum.EstimatedTime, result.DubiousResult );

            Result.RemoveDubiousReason(result, Result.DubiousResultEnum.DuplicateNumber);
            Assert.Equal(Result.DubiousResultEnum.EstimatedTime, result.DubiousResult);

            Result.RemoveDubiousReason(result, Result.DubiousResultEnum.EstimatedTime);
            Assert.Equal(Result.DubiousResultEnum.None, result.DubiousResult);
        }
    }
}
