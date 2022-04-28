namespace Avalanche.Localization.Pluralization;
using Avalanche.Utilities;
using System;
using System.Collections.Generic;

/// <summary>Shared plural rules</summary>
/// <see href="https://unicode-org.github.io/cldr/ldml/tr35-numbers.html#Plural_rules_syntax"/>
/// <see href="https://www.unicode.org/cldr/charts/33/supplemental/language_plural_rules.html"/>
/// <see href="http://cldr.unicode.org/index/cldr-spec/plural-rules"/>
/// <see href="https://unicode.org/Public/cldr/"/>  
/// <remarks>
/// UNICODE, INC. LICENSE AGREEMENT - DATA FILES AND SOFTWARE
/// 
/// See Terms of Use for definitions of Unicode Inc.'s
/// Data Files and Software.
/// 
/// NOTICE TO USER: Carefully read the following legal agreement.
/// BY DOWNLOADING, INSTALLING, COPYING OR OTHERWISE USING UNICODE INC.'S
/// DATA FILES ("DATA FILES"), AND/OR SOFTWARE ("SOFTWARE"),
/// YOU UNEQUIVOCALLY ACCEPT, AND AGREE TO BE BOUND BY, ALL OF THE
/// TERMS AND CONDITIONS OF THIS AGREEMENT.
/// IF YOU DO NOT AGREE, DO NOT DOWNLOAD, INSTALL, COPY, DISTRIBUTE OR USE
/// THE DATA FILES OR SOFTWARE.
/// 
/// COPYRIGHT AND PERMISSION NOTICE
/// 
/// Copyright © 1991-2022 Unicode, Inc. All rights reserved.
/// Distributed under the Terms of Use in https://www.unicode.org/copyright.html.
/// 
/// Permission is hereby granted, free of charge, to any person obtaining
/// a copy of the Unicode data files and any associated documentation
/// (the "Data Files") or Unicode software and any associated documentation
/// (the "Software") to deal in the Data Files or Software
/// without restriction, including without limitation the rights to use,
/// copy, modify, merge, publish, distribute, and/or sell copies of
/// the Data Files or Software, and to permit persons to whom the Data Files
/// or Software are furnished to do so, provided that either
/// (a) this copyright and permission notice appear with all copies
/// of the Data Files or Software, or
/// (b) this copyright and permission notice appear in associated
/// Documentation.
/// 
/// THE DATA FILES AND SOFTWARE ARE PROVIDED "AS IS", WITHOUT WARRANTY OF
/// ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
/// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
/// NONINFRINGEMENT OF THIRD PARTY RIGHTS.
/// IN NO EVENT SHALL THE COPYRIGHT HOLDER OR HOLDERS INCLUDED IN THIS
/// NOTICE BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT OR CONSEQUENTIAL
/// DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE,
/// DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER
/// TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
/// PERFORMANCE OF THE DATA FILES OR SOFTWARE.
/// 
/// Except as contained in this notice, the name of a copyright holder
/// shall not be used in advertising or otherwise to promote the sale,
/// use or other dealings in these Data Files or Software without prior
/// written authorization of the copyright holder.
/// </remarks>
public class CLDR41PluralRules : CLDRPluralRules
{
    /// <summary>Singleton</summary>
    static readonly Lazy<CLDR41PluralRules> instance = new Lazy<CLDR41PluralRules>(() => new CLDR41PluralRules().SetReadOnly());
    /// <summary>Singleton</summary>
    public new static CLDR41PluralRules Instance => instance.Value;

    /// <summary>Create CLDR plural rules</summary>
    public CLDR41PluralRules() : base("Unicode.CLDR41", 41, 859) => AllRules = AddTo(new List<IPluralRule>(859)).ToArray();

    /// <summary>Create and add plural rules to <paramref name="list"/>.</summary>
    public static IList<IPluralRule> AddTo(IList<IPluralRule> list)
    {
        // RuleSet name
        string ruleSet = "Unicode.CLDR41";
        // Get constants
        var c = CldrPluralConstants.Instance;
        
        //// cardinal ////
        // af, an, asa, az, bal, bem, bez, bg, brx, ce, cgg, chr, ckb, dv, ee, el, eo, eu, fo, fur, gsw, ha, haw, hu, jgo, jmc, ka, kaj, kcg, kk, kkj, kl, ks, ksb, ku, ky, lb, lg, mas, mgo, ml, mn, mr, nah, nb, nd, ne, nn, nnh, no, nr, ny, nyn, om, or, os, pap, ps, rm, rof, rwk, saq, sd, sdh, seh, sn, so, sq, ss, ssy, st, syr, ta, te, teo, tig, tk, tn, tr, ts, ug, uz, ve, vo, vun, wae, xh, xog
        foreach(string culture in c.c_b2cd700737655fc2)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: n=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E45218911E75E0E06, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // am, as, bn, doi, fa, gu, hi, kn, pcm, zu
        foreach(string culture in c.c_9b24c942e35258c5)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: i=0 or n=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E6A8CF657AB5B3530, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // ar, ars
        foreach(string culture in c.c_7a32677b76d337bb)
        {
            // zero: n=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", true), c.E45218911E75E0E07, null));
            // one: n=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E45218911E75E0E06, null));
            // two: n=2
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "two", true), c.E45218911E75E0E05, null));
            // few: n % 100=3..10
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.E7C3229095155F142, null));
            // many: n % 100=11..99
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "many", true), c.EEAEFB5ABF8D314C8, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // be
        foreach(string culture in c.c_af63bd4c8664b7bd)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: n % 10=1 and n % 100!=11
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.EAFE942E2E1B4A526, null));
            // few: n % 10=2..4 and n % 100!=12..14
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.E1977B78676F3918A, null));
            // many: n % 10=0 or n % 10=5..9 or n % 100=11..14
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "many", true), c.E9985DF47647FA8EA, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // br
        foreach(string culture in c.c_af63bd4c8673b7bd)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: n % 10=1 and n % 100!=11,71,91
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.EDF4E4F4D8E552609, null));
            // two: n % 10=2 and n % 100!=12,72,92
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "two", true), c.ED15FAB9B409A4308, null));
            // few: n % 10=3..4,9 and n % 100!=10..19,70..79,90..99
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.E8B30A7FD38122B53, null));
            // many: n!=0 and n % 1000000=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "many", true), c.E52E6239C3BEF79E1, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // bs, hr, sh, sr
        foreach(string culture in c.c_6e2d4af90830dcf9)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: v=0 and i % 10=1 and i % 100!=11 or f % 10=1 and f % 100!=11
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.EAC7ED1D2134174CC, null));
            // few: v=0 and i % 10=2..4 and i % 100!=12..14 or f % 10=2..4 and f % 100!=12..14
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.E4FA59408706AD2A4, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // cs, sk
        foreach(string culture in c.c_7932650874853407)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: i=1 and v=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.EAAC61CE8F3CDFA99, null));
            // few: i=2..4 and v=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.E7C13A65D879F3EDE, null));
            // many: v!=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "many", true), c.E4D7FC2DBABE81D9A, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // cy
        foreach(string culture in c.c_af63bd4c8678b7bc)
        {
            // zero: n=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", true), c.E45218911E75E0E07, null));
            // one: n=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E45218911E75E0E06, null));
            // two: n=2
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "two", true), c.E45218911E75E0E05, null));
            // few: n=3
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.E45218911E75E0E04, null));
            // many: n=6
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "many", true), c.E45218911E75E0E01, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // da
        foreach(string culture in c.c_af63bd4c8660b7bb)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: n=1 or t!=0 and i=0,1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.EF31AE578D91E6F40, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // ast, ca, de, en, et, fi, fy, gl, ia, io, ji, lij, nl, sc, scn, sv, sw, ur, yi
        foreach(string culture in c.c_d4ab421fdad8d880)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: i=1 and v=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.EAAC61CE8F3CDFA99, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // dsb, hsb
        foreach(string culture in c.c_7932316c749d32a9)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: v=0 and i % 100=1 or f % 100=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E85E9F19FF9A0EB83, null));
            // two: v=0 and i % 100=2 or f % 100=2
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "two", true), c.EC3C5111C890A468D, null));
            // few: v=0 and i % 100=3..4 or f % 100=3..4
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.E3F5CFE0B2A5A0BBF, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // es
        foreach(string culture in c.c_af63bd4c8672b7ba)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: n=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E45218911E75E0E06, null));
            // many: e=0 and i!=0 and i % 1000000=0 and v=0 or e!=0..5
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "many", true), c.E59E25F2B2F3A6AC4, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // ceb, fil, tl
        foreach(string culture in c.c_737faf248a437ae2)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: v=0 and i=1,2,3 or v=0 and i % 10!=4,6,9 or v!=0 and f % 10!=4,6,9
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.EC876BC1280793D21, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // fr
        foreach(string culture in c.c_af63bd4c8673b7b9)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: i=0,1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E74CC781520CCAA41, null));
            // many: e=0 and i!=0 and i % 1000000=0 and v=0 or e!=0..5
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "many", true), c.E59E25F2B2F3A6AC4, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // ga
        foreach(string culture in c.c_af63bd4c8660b7b8)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: n=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E45218911E75E0E06, null));
            // two: n=2
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "two", true), c.E45218911E75E0E05, null));
            // few: n=3..6
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.E58E4F1CCF73BC250, null));
            // many: n=7..10
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "many", true), c.E20EED0CD5228038B, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // gd
        foreach(string culture in c.c_af63bd4c8665b7b8)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: n=1,11
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.EDB0A62B7D620E99F, null));
            // two: n=2,12
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "two", true), c.EB84EF6C32B47E28C, null));
            // few: n=3..10,13..19
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.E7235D383CDEE4E29, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // gv
        foreach(string culture in c.c_af63bd4c8677b7b8)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: v=0 and i % 10=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E99F66DCEADF7C368, null));
            // two: v=0 and i % 10=2
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "two", true), c.E99F66DCEADF7C36B, null));
            // few: v=0 and i % 100=0,20,40,60,80
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.E352633E51903E5CC, null));
            // many: v!=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "many", true), c.E4D7FC2DBABE81D9A, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // he, iw
        foreach(string culture in c.c_6b3260085d532b9c)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: i=1 and v=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.EAAC61CE8F3CDFA99, null));
            // two: i=2 and v=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "two", true), c.E5DDD32BD695166C0, null));
            // many: v=0 and n!=0..10 and n % 10=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "many", true), c.E13FE60D69638CB98, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // ff, hy, kab
        foreach(string culture in c.c_83cbc75cf9bd33c2)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: i=0,1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E74CC781520CCAA41, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // bm, bo, dz, hnj, id, ig, ii, in, ja, jbo, jv, jw, kde, kea, km, ko, lkt, lo, ms, my, nqo, osa, sah, ses, sg, su, th, to, tpi, vi, wo, yo, yue, zh
        foreach(string culture in c.c_170a81363dcb957d)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // is
        foreach(string culture in c.c_af63bd4c8672b7b6)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: t=0 and i % 10=1 and i % 100!=11 or t!=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E6DFDA689335DBDB4, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // it, pt-PT
        foreach(string culture in c.c_ce51deaf11b0b2a2)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: i=1 and v=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.EAAC61CE8F3CDFA99, null));
            // many: e=0 and i!=0 and i % 1000000=0 and v=0 or e!=0..5
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "many", true), c.E59E25F2B2F3A6AC4, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // iu, naq, sat, se, sma, smi, smj, smn, sms
        foreach(string culture in c.c_13db252c758224dd)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: n=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E45218911E75E0E06, null));
            // two: n=2
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "two", true), c.E45218911E75E0E05, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // ksh
        foreach(string culture in c.c_af63bd248672b7b4)
        {
            // zero: n=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", true), c.E45218911E75E0E07, null));
            // one: n=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E45218911E75E0E06, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // kw
        foreach(string culture in c.c_af63bd4c8676b7b4)
        {
            // zero: n=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", true), c.E45218911E75E0E07, null));
            // one: n=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E45218911E75E0E06, null));
            // two: n % 100=2,22,42,62,82 or n % 1000=0 and n % 100000=1000..20000,40000,60000,80000 or n!=0 and n % 1000000=100000
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "two", true), c.E58A70F0789A08F5D, null));
            // few: n % 100=3,23,43,63,83
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.EE1D29B348E60C5F8, null));
            // many: n!=1 and n % 100=1,21,41,61,81
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "many", true), c.E1123C4216ED6652A, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // lag
        foreach(string culture in c.c_af63bd2b8660b7b3)
        {
            // zero: n=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", true), c.E45218911E75E0E07, null));
            // one: i=0,1 and n!=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E4301B6ED28CDFFCC, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // lt
        foreach(string culture in c.c_af63bd4c8675b7b3)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: n % 10=1 and n % 100!=11..19
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.EFE3DEDC13449AEAD, null));
            // few: n % 10=2..9 and n % 100!=11..19
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.E085C636984053ED5, null));
            // many: f!=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "many", true), c.ED24640F813396A2A, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // lv, prg
        foreach(string culture in c.c_7e325c6f7d1f2559)
        {
            // zero: n % 10=0 or n % 100=11..19 or v=2 and f % 100=11..19
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", true), c.EDF5D88E38D9E84C4, null));
            // one: n % 10=1 and n % 100!=11 or v=2 and f % 10=1 and f % 100!=11 or v!=2 and f % 10=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E5D7D7C6C74D8EA76, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // mk
        foreach(string culture in c.c_af63bd4c866ab7b2)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: v=0 and i % 10=1 and i % 100!=11 or f % 10=1 and f % 100!=11
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.EAC7ED1D2134174CC, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // mo, ro
        foreach(string culture in c.c_75325b086e4d2304)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: i=1 and v=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.EAAC61CE8F3CDFA99, null));
            // few: v!=0 or n=0 or n % 100=2..19
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.E47126BA004EB5E44, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // mt
        foreach(string culture in c.c_af63bd4c8675b7b2)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: n=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E45218911E75E0E06, null));
            // few: n=0 or n % 100=2..10
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.EDD607E469DC61B74, null));
            // many: n % 100=11..19
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "many", true), c.EEAEFB5ABF8D314C0, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // ak, bho, guw, ln, mg, nso, pa, ti, wa
        foreach(string culture in c.c_1936999b6e25d5d7)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: n=0..1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E82916D688A8D8D04, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // pl
        foreach(string culture in c.c_af63bd4c866db7af)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: i=1 and v=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.EAAC61CE8F3CDFA99, null));
            // few: v=0 and i % 10=2..4 and i % 100!=12..14
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.E8FD10B5797FCD594, null));
            // many: v=0 and i!=1 and i % 10=0..1 or v=0 and i % 10=5..9 or v=0 and i % 100=12..14
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "many", true), c.E12CD1058D5440019, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // pt
        foreach(string culture in c.c_af63bd4c8675b7af)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: i=0..1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.EAD1E1A42F8F288CB, null));
            // many: e=0 and i!=0 and i % 1000000=0 and v=0 or e!=0..5
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "many", true), c.E59E25F2B2F3A6AC4, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // ru, uk
        foreach(string culture in c.c_7b325608783f1a82)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: v=0 and i % 10=1 and i % 100!=11
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.EF167751ACA43A340, null));
            // few: v=0 and i % 10=2..4 and i % 100!=12..14
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.E8FD10B5797FCD594, null));
            // many: v=0 and i % 10=0 or v=0 and i % 10=5..9 or v=0 and i % 100=11..14
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "many", true), c.E717A09F9CB5D5495, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // shi
        foreach(string culture in c.c_af63bd258669b7ac)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: i=0 or n=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E6A8CF657AB5B3530, null));
            // few: n=2..10
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.E0318B7E92920D9B6, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // si
        foreach(string culture in c.c_af63bd4c8668b7ac)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: n=0,1 or i=0 and f=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.EF28E731584D454CF, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // sl
        foreach(string culture in c.c_af63bd4c866db7ac)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: v=0 and i % 100=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E276811FEADF7C368, null));
            // two: v=0 and i % 100=2
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "two", true), c.E276811FEADF7C36B, null));
            // few: v=0 and i % 100=3..4 or v!=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "few", true), c.E8B949D09B33A907A, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        // tzm
        foreach(string culture in c.c_af63bd21867bb7ab)
        {
            // zero: 
            list.Add(new PluralRule.Zero(new PluralRuleInfo(ruleSet, "cardinal", culture, "zero", false)));
            // one: n=0..1 or n=11..99
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "cardinal", culture, "one", true), c.E9446D8FF61D72123, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "cardinal", culture, "other", true)));
        }
        
        //// ordinal ////
        // af, am, an, ar, bg, bs, ce, cs, da, de, dsb, el, es, et, eu, fa, fi, fy, gl, gsw, he, hr, hsb, ia, id, in, is, iw, ja, km, kn, ko, ky, lt, lv, ml, mn, my, nb, nl, no, pa, pl, prg, ps, pt, ru, sd, sh, si, sk, sl, sr, sw, ta, te, th, tpi, tr, ur, uz, yue, zh, zu
        foreach(string culture in c.c_4a43f7aea6b9a065)
        {
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // as, bn
        foreach(string culture in c.c_79326708748037b8)
        {
            // one: n=1,5,7,8,9,10
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "one", true), c.E0A08DDF9CA52E4FC, null));
            // two: n=2,3
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "two", true), c.EB84EF6C32B75E28E, null));
            // few: n=4
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "few", true), c.E45218911E75E0E03, null));
            // many: n=6
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "many", true), c.E45218911E75E0E01, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // az
        foreach(string culture in c.c_af63bd4c867bb7be)
        {
            // one: i % 10=1,2,5,7,8 or i % 100=20,50,70,80
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "one", true), c.E5C23558094F23760, null));
            // few: i % 10=3,4 or i % 1000=100,200,300,400,500,600,700,800,900
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "few", true), c.E432B75D61B803E45, null));
            // many: i=0 or i % 10=6 or i % 100=40,60,90
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "many", true), c.E5816F306B8556A9E, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // bal, fil, fr, ga, hy, lo, mo, ms, ro, tl, vi
        foreach(string culture in c.c_a4952422b9a8fbba)
        {
            // one: n=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "one", true), c.E45218911E75E0E06, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // be
        foreach(string culture in c.c_af63bd4c8664b7bd)
        {
            // few: n % 10=2,3 and n % 100!=12,13
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "few", true), c.E7921EE27A11FCA07, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // ca
        foreach(string culture in c.c_af63bd4c8660b7bc)
        {
            // one: n=1,3
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "one", true), c.EDB0A62B7D611E99D, null));
            // two: n=2
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "two", true), c.E45218911E75E0E05, null));
            // few: n=4
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "few", true), c.E45218911E75E0E03, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // cy
        foreach(string culture in c.c_af63bd4c8678b7bc)
        {
            // zero: n=0,7,8,9
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "zero", true), c.EBD1431C13A0EDCE1, null));
            // one: n=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "one", true), c.E45218911E75E0E06, null));
            // two: n=2
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "two", true), c.E45218911E75E0E05, null));
            // few: n=3,4
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "few", true), c.E2C964F70516E3250, null));
            // many: n=5,6
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "many", true), c.EA243156208DA5994, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // en
        foreach(string culture in c.c_af63bd4c866fb7ba)
        {
            // one: n % 10=1 and n % 100!=11
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "one", true), c.EAFE942E2E1B4A526, null));
            // two: n % 10=2 and n % 100!=12
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "two", true), c.E5A87AA89272F955B, null));
            // few: n % 10=3 and n % 100!=13
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "few", true), c.EDB70DDB6F51DD874, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // gd
        foreach(string culture in c.c_af63bd4c8665b7b8)
        {
            // one: n=1,11
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "one", true), c.EDB0A62B7D620E99F, null));
            // two: n=2,12
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "two", true), c.EB84EF6C32B47E28C, null));
            // few: n=3,13
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "few", true), c.E2C964F70515D3255, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // gu, hi
        foreach(string culture in c.c_7b326108783d2dc0)
        {
            // one: n=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "one", true), c.E45218911E75E0E06, null));
            // two: n=2,3
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "two", true), c.EB84EF6C32B75E28E, null));
            // few: n=4
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "few", true), c.E45218911E75E0E03, null));
            // many: n=6
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "many", true), c.E45218911E75E0E01, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // hu
        foreach(string culture in c.c_af63bd4c8674b7b7)
        {
            // one: n=1,5
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "one", true), c.EDB0A62B7D611E99B, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // it, sc, scn
        foreach(string culture in c.c_6dc1ac09f850b130)
        {
            // many: n=11,8,80,800
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "many", true), c.EF23196F2AC58DB76, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // ka
        foreach(string culture in c.c_af63bd4c8660b7b4)
        {
            // one: i=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "one", true), c.EF9AEF3D0CA2E3A11, null));
            // many: i=0 or i % 100=2..20,40,60,80
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "many", true), c.E430E2E0A7A43E7DF, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // kk
        foreach(string culture in c.c_af63bd4c866ab7b4)
        {
            // many: n % 10=6 or n % 10=9 or n % 10=0 and n!=0
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "many", true), c.E20C7BB451ED5F2BA, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // kw
        foreach(string culture in c.c_af63bd4c8676b7b4)
        {
            // one: n=1..4 or n % 100=1..4,21..24,41..44,61..64,81..84
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "one", true), c.EF8C0EFEA67310E46, null));
            // many: n=5 or n % 100=5
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "many", true), c.E587FC86491928D0F, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // lij
        foreach(string culture in c.c_af63bd268668b7b3)
        {
            // many: n=11,8,80..89,800..899
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "many", true), c.E12960CA38DF20206, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // mk
        foreach(string culture in c.c_af63bd4c866ab7b2)
        {
            // one: i % 10=1 and i % 100!=11
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "one", true), c.E2F789E93A07A9788, null));
            // two: i % 10=2 and i % 100!=12
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "two", true), c.E7ED923FD8DE978AD, null));
            // many: i % 10=7,8 and i % 100!=17,18
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "many", true), c.E86706D7CAD333763, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // mr
        foreach(string culture in c.c_af63bd4c8673b7b2)
        {
            // one: n=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "one", true), c.E45218911E75E0E06, null));
            // two: n=2,3
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "two", true), c.EB84EF6C32B75E28E, null));
            // few: n=4
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "few", true), c.E45218911E75E0E03, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // ne
        foreach(string culture in c.c_af63bd4c8664b7b1)
        {
            // one: n=1..4
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "one", true), c.E61234B373B331F08, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // or
        foreach(string culture in c.c_af63bd4c8673b7b0)
        {
            // one: n=1,5,7..9
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "one", true), c.EB58FDC7F50486E42, null));
            // two: n=2,3
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "two", true), c.EB84EF6C32B75E28E, null));
            // few: n=4
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "few", true), c.E45218911E75E0E03, null));
            // many: n=6
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "many", true), c.E45218911E75E0E01, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // sq
        foreach(string culture in c.c_af63bd4c8670b7ac)
        {
            // one: n=1
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "one", true), c.E45218911E75E0E06, null));
            // many: n % 10=4 and n % 100!=14
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "many", true), c.E17B8CFD3B9FF8E41, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // sv
        foreach(string culture in c.c_af63bd4c8677b7ac)
        {
            // one: n % 10=1,2 and n % 100!=11,12
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "one", true), c.E1FE7B314E5788A69, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // tk
        foreach(string culture in c.c_af63bd4c866ab7ab)
        {
            // few: n % 10=6,9 or n=10
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "few", true), c.EED6C9733AF8EABBF, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // uk
        foreach(string culture in c.c_af63bd4c866ab7aa)
        {
            // few: n % 10=3 and n % 100!=13
            list.Add(new PluralRule.Expression(new PluralRuleInfo(ruleSet, "ordinal", culture, "few", true), c.EDB70DDB6F51DD874, null));
            // other: 
            list.Add(new PluralRule.True(new PluralRuleInfo(ruleSet, "ordinal", culture, "other", true)));
        }
        // Return
        return list;
    }
}
