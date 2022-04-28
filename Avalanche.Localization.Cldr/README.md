<b>Avalanche.Localization.Cldr</b> contains pluralization implementations that are derived from data files of Unicode CLDR, 
[[git]](https://github.com/tagcode/Avalanche.Localization/Avalanche.Localization.Cldr), 
[[www]](https://avalanche.fi/Avalanche.Core/Avalanche.Localization/docs/pluralization/cldrs.html), 
[[licensing]](https://avalanche.fi/Avalanche.Core/license/index.html).

There are following rulesets:

| IPluralRules | Property |
|:---------|:--------|
| Unicode.CLDR   | Avalanche.Localization.CLDRs.CLDR |
| Unicode.CLDR41 | [Avalanche.Localization.CLDRs.CLDR41](xref:Unicode.CLDR41)  |
| Unicode.CLDR40 | [Avalanche.Localization.CLDRs.CLDR40](xref:Unicode.CLDR40)  |

<details>
    <summary>Table is derived from plurals.xml and ordinals.xml of <a href="https://cldr.unicode.org/">Unicode CLDR</a>, and licensed as "Data files". Unicode CLDR is copyright of Unicode, Inc.</summary>
    <pre>
UNICODE, INC. LICENSE AGREEMENT - DATA FILES AND SOFTWARE

See Terms of Use for definitions of Unicode Inc.'s
Data Files and Software.

NOTICE TO USER: Carefully read the following legal agreement.
BY DOWNLOADING, INSTALLING, COPYING OR OTHERWISE USING UNICODE INC.'S
DATA FILES ("DATA FILES"), AND/OR SOFTWARE ("SOFTWARE"),
YOU UNEQUIVOCALLY ACCEPT, AND AGREE TO BE BOUND BY, ALL OF THE
TERMS AND CONDITIONS OF THIS AGREEMENT.
IF YOU DO NOT AGREE, DO NOT DOWNLOAD, INSTALL, COPY, DISTRIBUTE OR USE
THE DATA FILES OR SOFTWARE.

COPYRIGHT AND PERMISSION NOTICE

Copyright © 1991-2021 Unicode, Inc. All rights reserved.
Distributed under the Terms of Use in https://www.unicode.org/copyright.html.

Permission is hereby granted, free of charge, to any person obtaining
a copy of the Unicode data files and any associated documentation
(the "Data Files") or Unicode software and any associated documentation
(the "Software") to deal in the Data Files or Software
without restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, and/or sell copies of
the Data Files or Software, and to permit persons to whom the Data Files
or Software are furnished to do so, provided that either
(a) this copyright and permission notice appear with all copies
of the Data Files or Software, or
(b) this copyright and permission notice appear in associated
Documentation.

THE DATA FILES AND SOFTWARE ARE PROVIDED "AS IS", WITHOUT WARRANTY OF
ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT OF THIRD PARTY RIGHTS.
IN NO EVENT SHALL THE COPYRIGHT HOLDER OR HOLDERS INCLUDED IN THIS
NOTICE BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT OR CONSEQUENTIAL
DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE,
DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER
TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
PERFORMANCE OF THE DATA FILES OR SOFTWARE.

Except as contained in this notice, the name of a copyright holder
shall not be used in advertising or otherwise to promote the sale,
use or other dealings in these Data Files or Software without prior
written authorization of the copyright holder.
    </pre>
</details>

<br>

<b>CLDRs.CLDR</b> contains latest CLDR rules.

```csharp
IPluralRules rules = CLDRs.CLDR;
```

<b>CLDRs.CLDR40</b> CLDR40 rules.

```csharp
IPluralRules rules = CLDRs.CLDR40;
```

<b>CLDRs.All</b> contains all builtin CLDR rules.

```csharp
IPluralRules rules = CLDRs.All;
```

<b>CLDRs.PluralReader(filename, ruleset)</b> creates reader that reads <em>plurals.xml</em> and <em>ordinals.xml</em> files.

```csharp
IEnumerable<IPluralRule> reader = CLDRs.PluralReader("plurals.xml", "Unicode.CLDR40");
IPluralRules rules = new PluralRules(reader);
```

<b>CLDRs.PluralReaders(path)</b> creates reader that reads <em>pluralsXX.xml</em> and <em>ordinalsXX.xml</em> at a ''path''.

```csharp
IEnumerable<IPluralRule> reader = CLDRs.PluralReaders(".");
IPluralRules rules = new PluralRules(reader);
```

<b>CLDRs.All</b> can be queried for an evaluator, and evaluator for pluralization case.

```csharp
// Choose newest ruleset
string ruleset = PluralRuleInfo.NEWEST;
// Get 'de' rules evaluator for 'cardinal' values
IPluralRulesEvaluator evaluator = CLDRs.All.EvaluatorCached[(ruleset, "cardinal", "de", null, null)];
// Get culture
IFormatProvider culture = CultureInfo.GetCultureInfo("de");
// Create text
TextNumber textNumber = new TextNumber("100", culture);
// Evaluate plurality
IPluralRule[]? matches = evaluator.Evaluate<TextNumber>(textNumber);
// Get best matching plurality case
string? pluralityCase = matches?[0]?.Info.Case; // "other"
```


