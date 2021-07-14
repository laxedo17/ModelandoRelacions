//En OO, a herencia e algo normal, en programacion funcional non e nada recomendable
//Podemos crear algo equivalente usando record types con un referenciando ao outro
type Disco = { TamanhoGB: int }
//Definindo dous record types -- Ordenador e dependente de Disco
type Ordenador = 
    { Fabricante : string 
      Discos : Disco list }
//Creando unha instancia dun ordenador
let meuPC =
    { Fabricante = "Computas ben"
      Discos = 
      [ { TamanhoGB = 100 }
        { TamanhoGB = 250 }
        { TamanhoGB = 500 } ] }
// De moitos xeitos, esta mostra de código non é diferente de ter clases, cunha que ten unha
// propiedade que é unha instancia da segunda clase, agás que os records en F # son moito máis concisos e non caemos no enfoque dogmático dun ficheiro por clase. 

//Modelando unha xerarquia de types
// O problema é que ata agora non temos ningunha forma de modelar unha relación is-a en F #.
// Por exemplo, se desexas modelar diferentes tipos de discos duros no mundo de OO.... usaremos a herdanza aquí:
//  Un Disco Duro herda de Disco.
//  Almacenas campos compartidos na clase Disco.
//  Almacenas campos exclusivos do Disco Duro na clase de Disco Duro.
//  O comportamento común almacénase na clase Disco.
//  Permite anular comportamentos comúns a través do polimorfismo.  

// Pode que os teus datos comúns estean representados na clase Disco (como Fabricante
// e Tamaño), pero tes a velocidade RPM no Disco Duro e o número de pins en MMC. Do mesmo xeito, pode que teñas un método Seek () no Disco, que pode funcionar de forma significativamente diferente nos tres discos. Pero a capacidade de buscar un ficheiro é unha peza de funcionalidade común a todos os discos e, polo tanto, podería implementarse usando polimorfismo, tendo un método abstracto na clase base e, a continuación, substituíndo ese método na clase derivada.
// A continuación, os callers únense á clase base sen ter que preocuparse
// con que implementación están a tratar. 

//En F# existe algo par alidiar con este tipo de casos, usando...


//DISCRIMINATED UNIONS
//Podense ver de duas maneiras:
//  Como unha xerarquía de tipo normal, pero pechada. Con isto, quero dicir que ti defines
// todos os diferentes subtipos de maneira adiantada. Non podes declarar novos subtipos máis tarde.
//  Como unha forma de enums de estilo C #, pero coa capacidade de engadir metadatos a cada cas de enumeración

//Exemplo:
type Disco = //clase ou type base
| DiscoDuro of RPM:int * Pratos:int //subtipo Disco Duro contendo dous fields personalizados como metadatos
| EstadoSolido //sen custom fields
| MMC of NumeroDePins:int //MMC-custom field unico como metadatos

// Cada caso está separado polo símbolo de pipe (como no pattern matching). Se queres achegar metadatos específicos do cada caso, separa cada valor cun asterisco. Neste momento, paga a pena resaltar que modelaches o equivalente a toda unha xerarquía de tipos e clases en catro liñas de código. Compara isto co que normalmente farías en C # de forma convencional con xerarquía de clases:
// 1 Crea unha clase separada para o tipo de base e para cada subclase.
// 2 A mellor práctica (supostamente) impón que coloques cada subclase no seu propio ficheiro.
// 3 Crea un constructor para cada un, con campos e propiedades públicas axeitados. 

//Crear unha instancia dun caso DU é sinxelo:
// let instanciaDeClase = CasoDU (arg1, arg2, argn)

// Comezamos creando unha instancia dun disco duro con 250 RPM e sete pratos, seguido
// por un disco MMC con cinco pines. Finalmente, creamos un disco SSD. Porque este disco contén
// sen parámetros personalizados, podes eliminar completamente a "chamada ao constructor". 
let meuDiscoDuro = DiscoDuro (RPM = 250, Pratos = 7) //Argumentos nomeados explicitamente
let meuDiscoDuroVersionCorta = DiscoDuro(250, 7) //Sintaxis lixeira
let args = 250, 7
let meuDiscoDuroTupled = DiscoDuro args //Pasando todos os valores como un argumento simple, podemos omitir os parentesis e corcheas
let meu MMC = MMC 5
let meuSsd = EstadoSolido //Creando un caso de DU sen metadatos

//Accedendo a unha instancia da Discriminated Union con Pattern Matching
// Agora que creaches o teu DU, como o usas? Podes tentar entrar no meu disco duro e acceder a todos os campos. Se probas isto, decepcionaraste; non conseguirás ningunha
// propiedades. Isto é porque meuDiscoDuro é do tipo Disco e non DiscoDuro. O que cómpre facer
// de algunha maneira facer un unwrap de forma segura nun dos tres subtipos: DiscoDuro, EstadoSolido ou MMC.
// (É irrelevante que poidas ver no código que se trata realmente dun disco duro. En canto ao tipo, para o sistema, podería ser calquera dos tres.) Como o fas con seguridade? Ti
// usas o noso novo amigo da lección anterior: pattern matching.
// Supoñamos que desexas facer unha función que manexe o teu hipotético método Busqueda().
// Lembre que nunha xerarquía de OO, farías un método abstracto na clase base
// e proporcionas implementacións en cada caso. Noutras palabras, todas as implementacións
// vivirían co seu tipo asociado; non hai ningún lugar onde vexas todas as implementacións. En F #, adoptas un enfoque completamente diferente.

//Escribindo funcions para unha Discriminated Union
let busqueda disco =
    match disco with
    | DiscoDuro _ -> "Buscando ruidosamente a unha velocidade razonable" //fai match con calquera tipo de DiscoDuro
    | MMC _ -> "Buscando silenciosa pero lentamente" //fai match con calquera tipo de MMC
    | EstadoSolido -> "Xa atopei o que buscabas rapidamente"
busqueda meuSsd //devolve "Xa atopei o que buscabas rapidamente"

//Podemos millorar o codigo anterior para que faga match con Discos Duros cun RPM de 5400 e 5 pratos
let busquedaConValores disco =
    match disco with
    | DiscoDuro (5400, 5) -> "Buscando moi lentamente"
    // Coincidencia nun disco duro con 7 eixos e RPM de discriminated union para uso no RHS do caso 
    | DiscoDuro(rpm, 7) -> sprintf "Tenho 7 pratos e un numero de rpm de %d" rpm
    | MMC 3 -> "Buscando. Tenho 3 pins" //facendo match a un disco MMC de 3 pins 

//Practica: Funcion que fai pattern matching a unha discriminated union
//  1 Crea unha función, describir, que toma como parametro un disco duro.
// 2 A función debería devolver textos do seguinte xeito:
// a Se é un SSD, di: "Son un SSD de nova creación".
// b Se un MMC cun pin, di: "Só teño 1 pin".
// c Se un MMC con menos de cinco pins, di: "Eu son un MMC con algúns pins".
// d Se non, se é un MMC, di: "Son un MMC con <pin> pines".
// e Se ten un disco duro con 5400 RPM, di: "Son un disco duro lento".
// f Se o disco duro ten sete fusos, di: "Teño 7 eixos!"
// g Para calquera outro disco duro, di "Son un disco duro".
// 3 Usaremos o carácter comodín (_) para axudar a facer coincidencias parciais (por
// exemplo, (5400 RPM + calquera número de eixos) e cláusulas de garda coa
// palabra clave when.
// Usando comodíns con discriminated unions é tentador usar un comodín simple para o caso final no exemplo anterior. Pero ti sempre debes intentar ser o máis explícito posible cos casos de coincidencia (match cases) con discrininated unions. No exemplo anterior, e preferible indica DiscoDuro _ en vez de simplemente _.
// Así, se engades un novo tipo á unión discriminada (por exemplo, PenUSB), sempre recibirás avisos do compilador para recordarche que debes "xestionar" o novo caso.


//DISCRIMINATED UNIONS Anidadas (nested discriminated unions)
// Pode crear facilmente unións discriminadas aniñadas: un type de un type. Supoñamos que
// queres crear diferentes tipos de unidades MMC e facer unha coincidencia anidada niso.Como farías? Primeiro, crea o caso de DU aniñado e logo engádeo ao tipo orixinal
// da xerarquía como metadatos no caso principal (nesta situación, ese é o caso MMC de Disco)
type DiscoMMC = //DU aniñada con casos asociados
| RsMmc
| MmcPlus
| SecureMmc

type Disco = //Engandido a DU aniñada ao caso pai na DU de Disco
| MMC of DiscoMMC * NumeroDePins:int
//facendo matcho no DU de nivel superior e nos aniñados de maneira simultanea
match disco with
| MMC(MmcPlus, 3) -> "Buscando silenciosamente pero con seguridade"
| MMC(SecureMmc, 6) -> "Buscando silenciosamente con 6 pins"

//Shared fields. Campos compartidos
// Aínda non miramos como compartir campos comúns nun DU, por exemplo, o
// nome do fabricante dun disco duro ou o tamaño. Isto non é compatible coas DUs; non podes
// colocar campos comúns na base do DU (por exemplo, o tipo de disco). A mellor forma de
// logralo e usando unha combinación dun record e unha unión discriminada. Crea un rexistro de envoltura (wrapper) para manter calquera campo común, máis un
// campo mais que contén a unión discriminada: os distintos datos que varían.

//Codigo de inicio evolucionado
type InfoDoDisco =
    { Fabricante : string //Rexistro composto, comezando por campos comúns 
      TamanhoGB : int
      DatosDisco : Disco } //Datos que varias con campo como Discriminated Union

type Ordenador = { Fabricante : string; Discos : InfoDoDisco list} //record Ordenador conten o fabricante e unha lista de discos
let meuPC = 
    { Fabricante = "Computando" 
      Discos =
      [ { Fabricante = "DiscoTronCo" //Creando unha lista de discos usando a sintaxis []
          TamanhoGB = 100 
          DatosDisco = DiscoDuro (5400, 7) } //Campos comuns e DU variante como Disco Duro
        { Fabricante = "SuperRapido" 
          TamanhoGB = 250 
          DatosDisco = EstadoSolido } ] }

//Active patterns
// F # ten un mecanismo aínda máis poderoso e lixeiro para a clasificación de datos
// chamados patróns activos. Este é un tema máis avanzado. Podes definilos como unións discriminadas super avanzadas

// A norma rápida para cando usar Discriminated Unions é que, se precisas ter un conxunto extensible de subtipos abertos e pluggables que se poden engadir dinámicamente, as unións discriminadas non son moi adecuadas. As unións discriminadas son fixas no momento da compilación, polo que non podes conectar elementos novos facilmente.
// Para as DU con un gran número de casos (centos) que cambian rapidamente, tamén debes pensar ben. Cada vez que engadad un novo caso, o seu patrón coincidirá coa UD
// actualizado para manexar o novo subtipo (aínda que o compilador polo menos che indicará onde
// tes que actualizar o teu código!). En tal caso, pode ser un record ou funcións (metodos) en bruto serán un mellor axuste, ou retroceder a un modelo de herdanza baseado en clases.
// Pero se tes un conxunto fixo (ou que cambia lentamente) de casos, o que é apropiado a gran maioría de veces; entón un DU é moito mellor. Os DU son lixeiros, fáciles de usar e moi flexibles, xa que podes engadir novos comportamentos extremadamente
// rapido sen afectar o resto da base de código e obtés o beneficio do pattern matching. Tamén son moito máis fáciles de razoar; tendo todas as implementacións nun só lugar leva a un código moito máis fácil de entender

//Creando enums en F#
type Impresora = //Enum type
| Inkjet = 0 //Caso enum con valores ordinales explicitos
| Laserjet = 1
| DotMatrix = 2