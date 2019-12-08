using System.Collections.Generic;
using System.Linq;

namespace Suyeong.Core.Text.Diff
{
    public static class TextDiffer
    {
        const double SIMILAR_THRESHOLD = 0.65d;

        public static DiffResultViews DiffTexts(IEnumerable<string> mainTexts, IEnumerable<string> subTexts, out DiffResultDic resultDicMain, out DiffResultDic resultDicSub, double similarThreshold = SIMILAR_THRESHOLD)
        {
            resultDicMain = new DiffResultDic();
            resultDicSub = new DiffResultDic();

            Sentences mainSentences = ConvertSentences(mainTexts);
            Sentences subSentences = ConvertSentences(subTexts);

            GetDiffResultDic(mainSentences: mainSentences, subSentences: subSentences, similarLimit: similarThreshold, resultDicMain: out resultDicMain, resultDicSub: out resultDicSub);

            return ConvertResultToViews(mains: resultDicMain.GetValues(), subs: resultDicSub.GetValues());
        }

        static Sentences ConvertSentences(IEnumerable<string> texts)
        {
            Sentences sentences = new Sentences();

            int index = 0;

            foreach (string text in texts)
            {
                sentences.Add(new Sentence(index: index++, text: text.ToLower()));
            }

            return sentences;
        }

        static void GetDiffResultDic(Sentences mainSentences, Sentences subSentences, double similarLimit, out DiffResultDic resultDicMain, out DiffResultDic resultDicSub)
        {
            resultDicMain = new DiffResultDic();
            resultDicSub = new DiffResultDic();

            int lastIndex = -1, intersectCount;
            Sentence sub = new Sentence();
            List<string> sameTexts, modifiedTexts;
            bool find;

            foreach (Sentence main in mainSentences)
            {
                find = false;

                for (int i = lastIndex + 1; i < subSentences.Count; i++)
                {
                    sub = subSentences[i];

                    // 일단 동등한지 확인
                    if (string.Equals(main.Text, sub.Text))
                    {
                        resultDicMain.Add(main.Index, new DiffResult(index: main.Index, diffType: DiffType.Same, main: main, sub: sub, sameTexts: main.Texts.ToList(), modifiedTexts: new List<string>()));
                        resultDicSub.Add(sub.Index, new DiffResult(index: sub.Index, diffType: DiffType.Same, main: sub, sub: main, sameTexts: sub.Texts.ToList(), modifiedTexts: new List<string>()));

                        find = true;
                        lastIndex = i;
                        break;
                    }
                    // 동등하지 않은 상태에서 유사도 확인
                    else
                    {
                        // 갯수 확인 할 때는 교집합을 하지만, 실제 동일-수정된 텍스트를 찾을 때는 순서가 중요해서 교집합이나 차집합을 하지 않는다.
                        intersectCount = main.Texts.Intersect(sub.Texts).Count();

                        if ((double)(intersectCount * 2) / (double)(main.Texts.Length + sub.Texts.Length) > similarLimit)
                        {
                            GetSameAndModifiedTexts(mainTexts: main.Texts, subTexts: sub.Texts, sameTexts: out sameTexts, modifiedTexts: out modifiedTexts);
                            resultDicMain.Add(main.Index, new DiffResult(index: main.Index, diffType: DiffType.Modified, main: main, sub: sub, sameTexts: sameTexts, modifiedTexts: modifiedTexts));

                            GetSameAndModifiedTexts(mainTexts: sub.Texts, subTexts: main.Texts, sameTexts: out sameTexts, modifiedTexts: out modifiedTexts);
                            resultDicSub.Add(sub.Index, new DiffResult(index: sub.Index, diffType: DiffType.Modified, main: sub, sub: main, sameTexts: sameTexts, modifiedTexts: modifiedTexts));

                            find = true;
                            lastIndex = i;
                            break;
                        }
                    }
                }

                if (!find)
                {
                    resultDicMain.Add(main.Index, new DiffResult(index: main.Index, diffType: DiffType.Removed, main: main, sub: new Sentence(), sameTexts: new List<string>(), modifiedTexts: main.Texts.ToList()));
                }
            }

            // 위에서 처리하고 남은 right sentence는 add로 처리한다.
            foreach (Sentence sentence in subSentences)
            {
                if (!resultDicSub.ContainsKey(sentence.Index))
                {
                    resultDicSub.Add(sentence.Index, new DiffResult(index: sentence.Index, diffType: DiffType.Added, main: sentence, sub: new Sentence(), sameTexts: new List<string>(), modifiedTexts: sentence.Texts.ToList()));
                }
            }

            // 순서가 꼬였으므로 순서대로 정렬한다.
            resultDicSub = new DiffResultDic(resultDicSub.OrderBy(kvp => kvp.Key));
        }

        static void GetSameAndModifiedTexts(string[] mainTexts, string[] subTexts, out List<string> sameTexts, out List<string> modifiedTexts)
        {
            sameTexts = new List<string>();
            modifiedTexts = new List<string>();

            string main, sub;
            bool isEqual;
            int lastIndex = -1;

            for (int i = 0; i < mainTexts.Length; i++)
            {
                isEqual = false;
                main = mainTexts[i];

                for (int j = lastIndex + 1; j < subTexts.Length; j++)
                {
                    sub = subTexts[j];

                    if (string.Equals(main, sub))
                    {
                        isEqual = true;
                        lastIndex = i;
                        sameTexts.Add(main);
                        break;
                    }
                }

                if (!isEqual)
                {
                    modifiedTexts.Add(main);
                }
            }
        }

        static DiffResultViews ConvertResultToViews(DiffResults mains, DiffResults subs)
        {
            DiffResultViews views = new DiffResultViews();

            int index = 1, indexMain = 0, indexSub = 0;
            int count = mains.Count + subs.Count;
            DiffResult main, sub;

            for (int i = 0; i < count; i++)
            {
                if (indexMain < mains.Count && indexSub < subs.Count)
                {
                    main = mains[indexMain];
                    sub = subs[indexSub];

                    if (main.DiffType == DiffType.Removed)
                    {
                        views.Add(new DiffResultView(index: index++, mainResult: main, subResult: new DiffResult()));
                        indexMain++;
                    }
                    else if (sub.DiffType == DiffType.Added)
                    {
                        views.Add(new DiffResultView(index: index++, mainResult: new DiffResult(), subResult: sub));
                        indexSub++;
                    }
                    else
                    {
                        views.Add(new DiffResultView(index: index++, mainResult: main, subResult: sub));
                        indexMain++;
                        indexSub++;
                    }
                }
                else if (indexMain < mains.Count && indexSub >= subs.Count)
                {
                    main = mains[indexMain];

                    views.Add(new DiffResultView(index: index++, mainResult: main, subResult: new DiffResult()));
                    indexMain++;
                }
                else if (indexMain >= mains.Count && indexSub < subs.Count)
                {
                    sub = subs[indexSub];

                    views.Add(new DiffResultView(index: index++, mainResult: new DiffResult(), subResult: sub));
                    indexSub++;
                }
                else
                {
                    break;
                }
            }

            return views;
        }
    }
}
