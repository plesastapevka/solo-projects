import collections
import re
import lxml.etree as ET
import json
from lxml import objectify


def pre_process_text(text):
    clean_text = re.sub('<[^<]+>', "", text)
    raw_text = re.sub(r'[^A-Za-zČčŠšŽžĐđäöüßÄÖÜẞ. ]', '', clean_text)
    return raw_text.lower()


def create_n_grams(text, n=3):
    n_grams = [text[i:i + n] for i in range(len(text) - n + 1)]
    ngrams_freq = collections.Counter(n_grams)
    ngrams_map = ngrams_freq.most_common()
    return [i[0] for i in ngrams_map]


def get_ngrams(path_name="", string=False, input_text=""):
    if not string:
        text = open(path_name).read()
        raw_text = pre_process_text(text)
        bigrams = create_n_grams(raw_text, 2)
        trigrams = create_n_grams(raw_text, 3)
        fourgrams = create_n_grams(raw_text, 4)
        fivegrams = create_n_grams(raw_text, 5)
    else:
        raw_text = pre_process_text(input_text)
        bigrams = create_n_grams(raw_text, 2)
        trigrams = create_n_grams(raw_text, 3)
        fourgrams = create_n_grams(raw_text, 4)
        fivegrams = create_n_grams(raw_text, 5)

    return bigrams, trigrams, fourgrams, fivegrams


def get_diff(ngram, element, idx):
    try:
        index = ngram.index(element)
        return abs(index - idx)
    except ValueError:
        return len(ngram)


def detect_lang(bi, tri, four, five):
    bi_si, tri_si, four_si, five_si = get_ngrams("models/si.txt")
    bi_eng, tri_eng, four_eng, five_eng = get_ngrams("models/eng.txt")
    bi_ger, tri_ger, four_ger, five_ger = get_ngrams("models/ger.txt")
    sim_sum_si = 0
    sim_sum_eng = 0
    sim_sum_ger = 0

    # bigram processing
    for idx, e in enumerate(bi):
        diff_si = get_diff(bi_si, e, idx)
        diff_eng = get_diff(bi_eng, e, idx)
        diff_ger = get_diff(bi_ger, e, idx)
        if diff_si != -1:
            sim_sum_si += diff_si
        if diff_eng != -1:
            sim_sum_eng += diff_eng
        if diff_ger != -1:
            sim_sum_ger += diff_ger

    # trigram processing
    for idx, e in enumerate(tri):
        diff_si = get_diff(tri_si, e, idx)
        diff_eng = get_diff(tri_eng, e, idx)
        diff_ger = get_diff(tri_ger, e, idx)
        if diff_si != -1:
            sim_sum_si += diff_si
        if diff_eng != -1:
            sim_sum_eng += diff_eng
        if diff_ger != -1:
            sim_sum_ger += diff_ger

    # fourgram processing
    for idx, e in enumerate(four):
        diff_si = get_diff(four_si, e, idx)
        diff_eng = get_diff(four_eng, e, idx)
        diff_ger = get_diff(four_ger, e, idx)
        if diff_si != -1:
            sim_sum_si += diff_si
        if diff_eng != -1:
            sim_sum_eng += diff_eng
        if diff_ger != -1:
            sim_sum_ger += diff_ger

    # fivegram processing
    # for idx, e in enumerate(five):
    #     diff_si = get_diff(five_si, e, idx)
    #     diff_eng = get_diff(five_eng, e, idx)
    #     diff_ger = get_diff(five_ger, e, idx)
    #     if diff_si != -1:
    #         sim_sum_si += diff_si
    #     if diff_eng != -1:
    #         sim_sum_eng += diff_eng
    #     if diff_ger != -1:
    #         sim_sum_ger += diff_ger

    # print("English similarity sum is: " + str(sim_sum_eng))
    # print("Slovene similarity sum is: " + str(sim_sum_si))
    # print("German similarity sum is: " + str(sim_sum_ger))
    if min([sim_sum_si, sim_sum_eng, sim_sum_ger]) == sim_sum_eng:
        return "english"
    elif min([sim_sum_si, sim_sum_eng, sim_sum_ger]) == sim_sum_si:
        return "slovenian"
    elif min([sim_sum_si, sim_sum_eng, sim_sum_ger]) == sim_sum_ger:
        return "german"


def main(flag=False):
    if flag:
        bigrams, trigrams, fourgrams, fivegrams = get_ngrams("korpus/kas-5000.text.xml")
        print("Detecting language ...")
        detect_lang(bigrams, trigrams, fourgrams, fivegrams)
    else:
        xml_tree = ET.parse("korpus/kas-5000.text.xml")
        paragraphs = xml_tree.findall("page/p")

        for p in paragraphs:
            bigrams, trigrams, fourgrams, fivegrams = get_ngrams(input_text=p.text, string=True)
            lang = detect_lang(bigrams, trigrams, fourgrams, fivegrams)
            id = p.attrib.values()[0]
            print(id + ": " + lang)


if __name__ == "__main__":
    main()
