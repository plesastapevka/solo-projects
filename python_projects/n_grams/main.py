import collections
import re


def pre_process_text(path_name):
    text = re.sub('<[^<]+>', "", open(path_name).read())
    raw_text = re.sub(r'[^A-Za-zČčŠšŽžĐđäöüßÄÖÜẞ. ]', '', text)
    return raw_text.lower()


def create_n_grams(text, n=3):
    n_grams = [text[i:i + n] for i in range(len(text) - n + 1)]
    ngrams_freq = collections.Counter(n_grams)
    ngrams_map = ngrams_freq.most_common()
    return [i[0] for i in ngrams_map]


def get_ngrams(path_name):
    text = pre_process_text(path_name)
    bigrams = create_n_grams(text, 2)
    trigrams = create_n_grams(text, 3)
    fourgrams = create_n_grams(text, 4)
    fivegrams = create_n_grams(text, 5)
    return bigrams, trigrams, fourgrams, fivegrams


def detect_lang(bi, tri, four, five):
    # TODO: create ngrams for eng, ger and si lang
    bi_eng = ["e "]
    tri_eng = []
    four_eng = []
    five_eng = []
    sim_sum = 0
    for idx, bi_e in enumerate(bi):
        try:
            index = bi_eng.index(bi_e)
            sim_sum += abs(index - idx)
        except ValueError:
            pass
    print(sim_sum)


def main():
    bigrams, trigrams, fourgrams, fivegrams = get_ngrams("korpus/kas-4000.text.xml")
    print(list(bigrams))
    detect_lang(bigrams, trigrams, fourgrams, fivegrams)


if __name__ == "__main__":
    main()
