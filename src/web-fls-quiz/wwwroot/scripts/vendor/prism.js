/* PrismJS 1.17.1
https://prismjs.com/download.html#themes=prism&languages=clike+csharp */
var _self="undefined"!=typeof window?window:"undefined"!=typeof WorkerGlobalScope&&self instanceof WorkerGlobalScope?self:{},Prism=function(g){var c=/\blang(?:uage)?-([\w-]+)\b/i,a=0,C={manual:g.Prism&&g.Prism.manual,disableWorkerMessageHandler:g.Prism&&g.Prism.disableWorkerMessageHandler,util:{encode:function(e){return e instanceof M?new M(e.type,C.util.encode(e.content),e.alias):Array.isArray(e)?e.map(C.util.encode):e.replace(/&/g,"&amp;").replace(/</g,"&lt;").replace(/\u00a0/g," ")},type:function(e){return Object.prototype.toString.call(e).slice(8,-1)},objId:function(e){return e.__id||Object.defineProperty(e,"__id",{value:++a}),e.__id},clone:function n(e,t){var r,a,i=C.util.type(e);switch(t=t||{},i){case"Object":if(a=C.util.objId(e),t[a])return t[a];for(var o in r={},t[a]=r,e)e.hasOwnProperty(o)&&(r[o]=n(e[o],t));return r;case"Array":return a=C.util.objId(e),t[a]?t[a]:(r=[],t[a]=r,e.forEach(function(e,a){r[a]=n(e,t)}),r);default:return e}}},languages:{extend:function(e,a){var n=C.util.clone(C.languages[e]);for(var t in a)n[t]=a[t];return n},insertBefore:function(n,e,a,t){var r=(t=t||C.languages)[n],i={};for(var o in r)if(r.hasOwnProperty(o)){if(o==e)for(var l in a)a.hasOwnProperty(l)&&(i[l]=a[l]);a.hasOwnProperty(o)||(i[o]=r[o])}var s=t[n];return t[n]=i,C.languages.DFS(C.languages,function(e,a){a===s&&e!=n&&(this[e]=i)}),i},DFS:function e(a,n,t,r){r=r||{};var i=C.util.objId;for(var o in a)if(a.hasOwnProperty(o)){n.call(a,o,a[o],t||o);var l=a[o],s=C.util.type(l);"Object"!==s||r[i(l)]?"Array"!==s||r[i(l)]||(r[i(l)]=!0,e(l,n,o,r)):(r[i(l)]=!0,e(l,n,null,r))}}},plugins:{},highlightAll:function(e,a){C.highlightAllUnder(document,e,a)},highlightAllUnder:function(e,a,n){var t={callback:n,selector:'code[class*="language-"], [class*="language-"] code, code[class*="lang-"], [class*="lang-"] code'};C.hooks.run("before-highlightall",t);for(var r,i=e.querySelectorAll(t.selector),o=0;r=i[o++];)C.highlightElement(r,!0===a,t.callback)},highlightElement:function(e,a,n){for(var t,r="none",i=e;i&&!c.test(i.className);)i=i.parentNode;i&&(r=(i.className.match(c)||[,"none"])[1].toLowerCase(),t=C.languages[r]),e.className=e.className.replace(c,"").replace(/\s+/g," ")+" language-"+r,e.parentNode&&(i=e.parentNode,/pre/i.test(i.nodeName)&&(i.className=i.className.replace(c,"").replace(/\s+/g," ")+" language-"+r));var o={element:e,language:r,grammar:t,code:e.textContent},l=function(e){o.highlightedCode=e,C.hooks.run("before-insert",o),o.element.innerHTML=o.highlightedCode,C.hooks.run("after-highlight",o),C.hooks.run("complete",o),n&&n.call(o.element)};if(C.hooks.run("before-sanity-check",o),o.code)if(C.hooks.run("before-highlight",o),o.grammar)if(a&&g.Worker){var s=new Worker(C.filename);s.onmessage=function(e){l(e.data)},s.postMessage(JSON.stringify({language:o.language,code:o.code,immediateClose:!0}))}else l(C.highlight(o.code,o.grammar,o.language));else l(C.util.encode(o.code));else C.hooks.run("complete",o)},highlight:function(e,a,n){var t={code:e,grammar:a,language:n};return C.hooks.run("before-tokenize",t),t.tokens=C.tokenize(t.code,t.grammar),C.hooks.run("after-tokenize",t),M.stringify(C.util.encode(t.tokens),t.language)},matchGrammar:function(e,a,n,t,r,i,o){for(var l in n)if(n.hasOwnProperty(l)&&n[l]){if(l==o)return;var s=n[l];s="Array"===C.util.type(s)?s:[s];for(var g=0;g<s.length;++g){var c=s[g],u=c.inside,h=!!c.lookbehind,f=!!c.greedy,d=0,m=c.alias;if(f&&!c.pattern.global){var p=c.pattern.toString().match(/[imuy]*$/)[0];c.pattern=RegExp(c.pattern.source,p+"g")}c=c.pattern||c;for(var y=t,v=r;y<a.length;v+=a[y].length,++y){var k=a[y];if(a.length>e.length)return;if(!(k instanceof M)){if(f&&y!=a.length-1){if(c.lastIndex=v,!(x=c.exec(e)))break;for(var b=x.index+(h?x[1].length:0),w=x.index+x[0].length,A=y,P=v,O=a.length;A<O&&(P<w||!a[A].type&&!a[A-1].greedy);++A)(P+=a[A].length)<=b&&(++y,v=P);if(a[y]instanceof M)continue;N=A-y,k=e.slice(v,P),x.index-=v}else{c.lastIndex=0;var x=c.exec(k),N=1}if(x){h&&(d=x[1]?x[1].length:0);w=(b=x.index+d)+(x=x[0].slice(d)).length;var j=k.slice(0,b),S=k.slice(w),E=[y,N];j&&(++y,v+=j.length,E.push(j));var _=new M(l,u?C.tokenize(x,u):x,m,x,f);if(E.push(_),S&&E.push(S),Array.prototype.splice.apply(a,E),1!=N&&C.matchGrammar(e,a,n,y,v,!0,l),i)break}else if(i)break}}}}},tokenize:function(e,a){var n=[e],t=a.rest;if(t){for(var r in t)a[r]=t[r];delete a.rest}return C.matchGrammar(e,n,a,0,0,!1),n},hooks:{all:{},add:function(e,a){var n=C.hooks.all;n[e]=n[e]||[],n[e].push(a)},run:function(e,a){var n=C.hooks.all[e];if(n&&n.length)for(var t,r=0;t=n[r++];)t(a)}},Token:M};function M(e,a,n,t,r){this.type=e,this.content=a,this.alias=n,this.length=0|(t||"").length,this.greedy=!!r}if(g.Prism=C,M.stringify=function(e,a){if("string"==typeof e)return e;if(Array.isArray(e))return e.map(function(e){return M.stringify(e,a)}).join("");var n={type:e.type,content:M.stringify(e.content,a),tag:"span",classes:["token",e.type],attributes:{},language:a};if(e.alias){var t=Array.isArray(e.alias)?e.alias:[e.alias];Array.prototype.push.apply(n.classes,t)}C.hooks.run("wrap",n);var r=Object.keys(n.attributes).map(function(e){return e+'="'+(n.attributes[e]||"").replace(/"/g,"&quot;")+'"'}).join(" ");return"<"+n.tag+' class="'+n.classes.join(" ")+'"'+(r?" "+r:"")+">"+n.content+"</"+n.tag+">"},!g.document)return g.addEventListener&&(C.disableWorkerMessageHandler||g.addEventListener("message",function(e){var a=JSON.parse(e.data),n=a.language,t=a.code,r=a.immediateClose;g.postMessage(C.highlight(t,C.languages[n],n)),r&&g.close()},!1)),C;var e=document.currentScript||[].slice.call(document.getElementsByTagName("script")).pop();return e&&(C.filename=e.src,C.manual||e.hasAttribute("data-manual")||("loading"!==document.readyState?window.requestAnimationFrame?window.requestAnimationFrame(C.highlightAll):window.setTimeout(C.highlightAll,16):document.addEventListener("DOMContentLoaded",C.highlightAll))),C}(_self);"undefined"!=typeof module&&module.exports&&(module.exports=Prism),"undefined"!=typeof global&&(global.Prism=Prism);
Prism.languages.clike={comment:[{pattern:/(^|[^\\])\/\*[\s\S]*?(?:\*\/|$)/,lookbehind:!0},{pattern:/(^|[^\\:])\/\/.*/,lookbehind:!0,greedy:!0}],string:{pattern:/(["'])(?:\\(?:\r\n|[\s\S])|(?!\1)[^\\\r\n])*\1/,greedy:!0},"class-name":{pattern:/((?:\b(?:class|interface|extends|implements|trait|instanceof|new)\s+)|(?:catch\s+\())[\w.\\]+/i,lookbehind:!0,inside:{punctuation:/[.\\]/}},keyword:/\b(?:if|else|while|do|for|return|in|instanceof|function|new|try|throw|catch|finally|null|break|continue)\b/,boolean:/\b(?:true|false)\b/,function:/\w+(?=\()/,number:/\b0x[\da-f]+\b|(?:\b\d+\.?\d*|\B\.\d+)(?:e[+-]?\d+)?/i,operator:/--?|\+\+?|!=?=?|<=?|>=?|==?=?|&&?|\|\|?|\?|\*|\/|~|\^|%/,punctuation:/[{}[\];(),.:]/};
Prism.languages.csharp=Prism.languages.extend("clike",{keyword:/\b(?:abstract|add|alias|as|ascending|async|await|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|descending|do|double|dynamic|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|from|get|global|goto|group|if|implicit|in|int|interface|internal|into|is|join|let|lock|long|namespace|new|null|object|operator|orderby|out|override|params|partial|private|protected|public|readonly|ref|remove|return|sbyte|sealed|select|set|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|value|var|virtual|void|volatile|where|while|yield)\b/,string:[{pattern:/@("|')(?:\1\1|\\[\s\S]|(?!\1)[^\\])*\1/,greedy:!0},{pattern:/("|')(?:\\.|(?!\1)[^\\\r\n])*?\1/,greedy:!0}],"class-name":[{pattern:/\b[A-Z]\w*(?:\.\w+)*\b(?=\s+\w+)/,inside:{punctuation:/\./}},{pattern:/(\[)[A-Z]\w*(?:\.\w+)*\b/,lookbehind:!0,inside:{punctuation:/\./}},{pattern:/(\b(?:class|interface)\s+[A-Z]\w*(?:\.\w+)*\s*:\s*)[A-Z]\w*(?:\.\w+)*\b/,lookbehind:!0,inside:{punctuation:/\./}},{pattern:/((?:\b(?:class|interface|new)\s+)|(?:catch\s+\())[A-Z]\w*(?:\.\w+)*\b/,lookbehind:!0,inside:{punctuation:/\./}}],number:/\b0x[\da-f]+\b|(?:\b\d+\.?\d*|\B\.\d+)f?/i,operator:/>>=?|<<=?|[-=]>|([-+&|?])\1|~|[-+*/%&|^!=<>]=?/,punctuation:/\?\.?|::|[{}[\];(),.:]/}),Prism.languages.insertBefore("csharp","class-name",{"generic-method":{pattern:/\w+\s*<[^>\r\n]+?>\s*(?=\()/,inside:{function:/^\w+/,"class-name":{pattern:/\b[A-Z]\w*(?:\.\w+)*\b/,inside:{punctuation:/\./}},keyword:Prism.languages.csharp.keyword,punctuation:/[<>(),.:]/}},preprocessor:{pattern:/(^\s*)#.*/m,lookbehind:!0,alias:"property",inside:{directive:{pattern:/(\s*#)\b(?:define|elif|else|endif|endregion|error|if|line|pragma|region|undef|warning)\b/,lookbehind:!0,alias:"keyword"}}}}),Prism.languages.dotnet=Prism.languages.cs=Prism.languages.csharp;
