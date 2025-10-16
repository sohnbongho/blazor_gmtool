using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Helper
{
    public sealed class UIDGenerator
    {
        private static readonly Lazy<UIDGenerator> lazy = new Lazy<UIDGenerator>(() => new UIDGenerator());

        public static UIDGenerator Instance { get { return lazy.Value; } }

        private const int MaxWorldIndex = 255;  // 최대 256개의 world를 지원
        private const ulong WorldIdMask = 0xFF00000000000000; // 최상위 8비트를 world index로 사용
        private const ulong TimeIdMask = 0x00FFFFFFFFFFFFFF;  // 나머지 56비트를 시간 기반 UID로 사용

        private static readonly object lockObject = new object();
        private static ulong lastId = 0;

        public ulong GetNextUID(int worldIndex)
        {
            if (worldIndex < 0 || worldIndex > MaxWorldIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(worldIndex), "Invalid world index.");
            }

            lock (lockObject)
            {
                ulong currentTimeBasedId = (ulong)DateTime.UtcNow.Ticks & TimeIdMask;

                // 이전 ID와 동일한 시간에 여러 UID가 요청될 경우를 대비해 증가시킨다.
                ulong newIdWithoutWorld = (currentTimeBasedId <= (lastId & TimeIdMask))
                    ? (lastId & TimeIdMask) + 1
                    : currentTimeBasedId;

                lastId = newIdWithoutWorld;
                ulong worldBasedId = ((ulong)worldIndex << 56) & WorldIdMask;  // World index를 최상위 비트로 이동

                return worldBasedId | newIdWithoutWorld;
            }
        }
    }


}
