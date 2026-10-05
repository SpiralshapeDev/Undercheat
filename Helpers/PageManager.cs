namespace UnderCheat.Helpers
{
    internal static class PageManager
    {
        public static int currentPage = 1;
        private static readonly int maxPage = 4; 
        public static int discoverPageItemIndex = 0;
        public static int nextPage => WrapIndex(currentPage + 1,1,maxPage);
        
        public static void Reset()
        {
            currentPage = 1;
            discoverPageItemIndex = 0;
        }
        
        public static void Next()
        {
            currentPage = WrapIndex(currentPage + 1,1,maxPage);
        }
        
        public static int WrapIndex(int index, int minIndex, int maxIndex)
        {
            if (index < minIndex) return maxIndex;
            if (index > maxIndex) return minIndex;
            
            return index;
        }

    }
}