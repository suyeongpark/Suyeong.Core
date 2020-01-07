using System;
using System.Collections.Generic;
using System.Linq;

namespace Suyeong.Core.Algorithm.DocDetector
{
    public static class TextSetDetector
    {
        const double SIZE_RATIO = 0.5d;
        const double ALIGN_RATIO = 0.1d;
        const double SPACING_RATIO = 1.2d;

        /// <summary>
        /// 컬럼을 나누는 것은 MergeTextLine로 line을 가져온 후에 해야 한다.
        /// MergeTextsGroup은 도면 같이 아예 줄 같은게 없는 곳에서 뭉쳐진 텍스트틀 합하는 것.
        /// </summary>

        public static TextBlockSetCollection MergeTextLineByHorizontal(TextBlockCollection sources, double sizeRatio = SIZE_RATIO, double alignRatio = ALIGN_RATIO)
        {
            TextBlockSetCollection lineTexts = new TextBlockSetCollection();

            TextBlockCollection textsLine;
            TextBlock main, sub;

            Dictionary<int, int> duplicateDic = new Dictionary<int, int>();
            int size, align, index = 0;

            for (int i = 0; i < sources.Count; i++)
            {
                if (!duplicateDic.ContainsKey(i))
                {
                    main = sources[i];

                    size = (int)Math.Round(main.Height * sizeRatio, MidpointRounding.AwayFromZero);
                    align = (int)Math.Round(main.Height * alignRatio, MidpointRounding.AwayFromZero);

                    textsLine = new TextBlockCollection();
                    textsLine.Add(main);

                    duplicateDic.Add(i, i);

                    for (int j = i + 1; j < sources.Count; j++)
                    {
                        sub = sources[j];

                        if (!duplicateDic.ContainsKey(j))
                        {
                            if (IsSameFontSizeByHorizontal(blockA: main, blockB: sub, size: size) &&
                                IsAlignByHorizontal(blockA: main, blockB: sub, align: align))
                            {
                                textsLine.Add(sub);
                                duplicateDic.Add(j, j);
                            }
                        }
                    }

                    lineTexts.Add(new TextBlockSet(index: index++, textBlocks: new TextBlockCollection(textsLine.OrderBy(text => text.LeftX))));
                }
            }

            return lineTexts;
        }

        public static TextBlockSetCollection MergeTextLineByVertical(TextBlockCollection sources, bool isBottomFirst = true, double sizeRatio = SIZE_RATIO, double alignRatio = ALIGN_RATIO)
        {
            TextBlockSetCollection lineTexts = new TextBlockSetCollection();

            TextBlockCollection textsLine, textsLineOrder;
            TextBlock main, sub;

            Dictionary<int, int> duplicateDic = new Dictionary<int, int>();
            int size, align, index = 0;

            for (int i = 0; i < sources.Count; i++)
            {
                if (!duplicateDic.ContainsKey(i))
                {
                    main = sources[i];

                    size = (int)Math.Round(main.Width * sizeRatio, MidpointRounding.AwayFromZero);
                    align = (int)Math.Round(main.Width * alignRatio, MidpointRounding.AwayFromZero);

                    textsLine = new TextBlockCollection();
                    textsLine.Add(main);

                    duplicateDic.Add(i, i);

                    for (int j = i + 1; j < sources.Count; j++)
                    {
                        sub = sources[j];

                        if (!duplicateDic.ContainsKey(j))
                        {
                            if (IsSameFontSizeByVertical(blockA: main, blockB: sub, size: size) &&
                                IsAlignByVertical(blockA: main, blockB: sub, align: align))
                            {
                                textsLine.Add(sub);
                                duplicateDic.Add(j, j);
                            }
                        }
                    }

                    textsLineOrder = isBottomFirst ? new TextBlockCollection(textsLine.OrderBy(text => text.BottomY)) : new TextBlockCollection(textsLine.OrderBy(text => text.TopY));
                    lineTexts.Add(new TextBlockSet(index: index++, textBlocks: textsLineOrder));
                }
            }

            return lineTexts;
        }

        public static TextBlockSetCollection MergeTextGroupByHorizontal(TextBlockCollection sources, double sizeRatio = SIZE_RATIO, double alignRatio = ALIGN_RATIO, double spacingRatio = SPACING_RATIO)
        {
            TextBlockSetCollection groupTexts = new TextBlockSetCollection();

            TextBlockCollection texts;

            Dictionary<int, int> duplicateDic = new Dictionary<int, int>();

            foreach (TextBlock source in sources)
            {
                if (!duplicateDic.ContainsKey(source.Index))
                {
                    texts = new TextBlockCollection();
                    texts.Add(source);

                    duplicateDic.Add(source.Index, source.Index);

                    FindHorizontalTextsRecursive(current: source, sizeRatio: sizeRatio, alignRatio: alignRatio, spacingRatio: spacingRatio, texts: ref texts, sources: ref sources, duplicateDic: ref duplicateDic);

                    groupTexts.Add(new TextBlockSet(index: groupTexts.Count, textBlocks: new TextBlockCollection(texts.OrderBy(text => text.LeftX))));
                }
            }

            return groupTexts;
        }

        public static TextBlockSetCollection MergeTextGroupByVertical(TextBlockCollection sources, bool isBottomFirst = true, double sizeRatio = SIZE_RATIO, double alignRatio = ALIGN_RATIO, double spacingRatio = SPACING_RATIO)
        {
            TextBlockSetCollection groupTexts = new TextBlockSetCollection();

            TextBlockCollection texts, textsOrder;

            Dictionary<int, int> duplicateDic = new Dictionary<int, int>();

            foreach (TextBlock source in sources)
            {
                if (!duplicateDic.ContainsKey(source.Index))
                {
                    texts = new TextBlockCollection();
                    texts.Add(source);

                    duplicateDic.Add(source.Index, source.Index);

                    FindVerticalTextsRecursive(current: source, sizeRatio: sizeRatio, alignRatio: alignRatio, spacingRatio: spacingRatio, texts: ref texts, sources: ref sources, duplicateDic: ref duplicateDic);

                    textsOrder = isBottomFirst ? new TextBlockCollection(texts.OrderBy(text => text.BottomY)) : new TextBlockCollection(texts.OrderBy(text => text.TopY));
                    groupTexts.Add(new TextBlockSet(index: groupTexts.Count, textBlocks: textsOrder));
                }
            }

            return groupTexts;
        }

        static void FindHorizontalTextsRecursive(TextBlock current, double sizeRatio, double alignRatio, double spacingRatio, ref TextBlockCollection texts, ref TextBlockCollection sources, ref Dictionary<int, int> duplicateDic)
        {
            int size = (int)Math.Round(current.Height * sizeRatio, MidpointRounding.AwayFromZero);
            int align = (int)Math.Round(current.Height * alignRatio, MidpointRounding.AwayFromZero);
            int spacing = (int)Math.Round(current.Height * spacingRatio, MidpointRounding.AwayFromZero);

            foreach (TextBlock source in sources)
            {
                if (!duplicateDic.ContainsKey(source.Index) &&
                    IsSameFontSizeByHorizontal(blockA: current, blockB: source, size: size) &&
                    IsAlignByHorizontal(blockA: current, blockB: source, align: align) &&
                    IsCloseTextByHorizontal(blockA: current, blockB: source, spacing: spacing))
                {
                    texts.Add(source);
                    duplicateDic.Add(source.Index, source.Index);

                    FindHorizontalTextsRecursive(current: source, sizeRatio: sizeRatio, alignRatio: alignRatio, spacingRatio: spacingRatio, texts: ref texts, sources: ref sources, duplicateDic: ref duplicateDic);
                }
            }
        }

        static void FindVerticalTextsRecursive(TextBlock current, double sizeRatio, double alignRatio, double spacingRatio, ref TextBlockCollection texts, ref TextBlockCollection sources, ref Dictionary<int, int> duplicateDic)
        {
            int size = (int)Math.Round(current.Width * sizeRatio, MidpointRounding.AwayFromZero);
            int align = (int)Math.Round(current.Width * alignRatio, MidpointRounding.AwayFromZero);
            int spacing = (int)Math.Round(current.Width * spacingRatio, MidpointRounding.AwayFromZero);

            foreach (TextBlock source in sources)
            {
                if (!duplicateDic.ContainsKey(source.Index) &&
                    IsSameFontSizeByVertical(blockA: current, blockB: source, size: size) &&
                    IsAlignByVertical(blockA: current, blockB: source, align: align) &&
                    IsCloseTextByVertical(blockA: current, blockB: source, spacing: spacing))
                {
                    texts.Add(source);
                    duplicateDic.Add(source.Index, source.Index);

                    FindVerticalTextsRecursive(current: source, sizeRatio: sizeRatio, alignRatio: alignRatio, spacingRatio: spacingRatio, texts: ref texts, sources: ref sources, duplicateDic: ref duplicateDic);
                }
            }
        }

        static bool IsSameFontSizeByHorizontal(TextBlock blockA, TextBlock blockB, int size)
        {
            return Math.Abs(blockA.Height - blockB.Height) < size;
        }

        static bool IsSameFontSizeByVertical(TextBlock blockA, TextBlock blockB, int size)
        {
            return Math.Abs(blockA.Width - blockB.Width) < size;
        }

        static bool IsAlignByHorizontal(TextBlock blockA, TextBlock blockB, int align)
        {
            // line을 찾을 때는 current topY < source bottomY && current bottomY > source topY 이면 충분했지만, line이 없는 문서에서는 정렬을 봐야 한다.
            // 정렬이 되었으면 같은 단어로 본다.
            return Math.Abs(blockA.BottomY - blockB.BottomY) < align || Math.Abs(blockA.CenterY - blockB.CenterY) < align || Math.Abs(blockA.TopY - blockB.TopY) < align;
        }

        static bool IsAlignByVertical(TextBlock blockA, TextBlock blockB, int align)
        {
            return Math.Abs(blockA.LeftX - blockB.LeftX) < align || Math.Abs(blockA.CenterX - blockB.CenterX) < align || Math.Abs(blockA.RightX - blockB.RightX) < align;
        }

        static bool IsCloseTextByHorizontal(TextBlock blockA, TextBlock blockB, int spacing)
        {
            return Math.Abs(blockA.LeftX - blockB.RightX) < spacing || Math.Abs(blockA.RightX - blockB.LeftX) < spacing;
        }

        static bool IsCloseTextByVertical(TextBlock blockA, TextBlock blockB, int spacing)
        {
            return Math.Abs(blockA.TopY - blockB.BottomY) < spacing || Math.Abs(blockA.BottomY - blockB.TopY) < spacing;
        }
    }
}
