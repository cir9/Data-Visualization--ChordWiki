const _note_index_arr = [...[...Array(7).keys()].map(i=>i+1)]

const _note_index_map = {
    'b1' : 0,  '1' : 0,  '#1' : 0,
    'b2' : 1,  '2' : 1,  '#2' : 1,
    'b3' : 2,  '3' : 2,  '#3' : 2,
    'b4' : 3,  '4' : 3,  '#4' : 3,
    'b5' : 4,  '5' : 4,  '#5' : 4,
    'b6' : 5,  '6' : 5,  '#6' : 5,
    'b7' : 6,  '7' : 6,  '#7' : 6,
}

const _chord_name_map = {
    'bI'   : 11, 'I'   : 0,  '#I'   : 1,
    'bII'  : 1,  'II'  : 2,  '#II'  : 3,
    'bIII' : 3,  'III' : 4,  '#III' : 5,
    'bIV'  : 4,  'IV'  : 5,  '#IV'  : 6,
    'bV'   : 6,  'V'   : 7,  '#V'   : 8,
    'bVI'  : 8,  'VI'  : 9,  '#VI'  : 10,
    'bVII' : 10, 'VII' : 11, '#VII' : 0,
}
const _note_name_map = {
    'b1' : 11, '1' : 0,  '#1' : 1,
    'b2' : 1,  '2' : 2,  '#2' : 3,
    'b3' : 3,  '3' : 4,  '#3' : 5,
    'b4' : 4,  '4' : 5,  '#4' : 6,
    'b5' : 6,  '5' : 7,  '#5' : 8,
    'b6' : 8,  '6' : 9,  '#6' : 10,
    'b7' : 10, '7' : 11, '#7' : 0,
}

const _tension_map = {
    'P1' : 0,
    'm2' : 1,   'M2': 2,
    'm3' : 3,   'M3': 4,
    'd4' : 4,   'P4': 5,    'A4' : 6,
    'd5' : 6,   'P5': 7,    'A5' : 8,
    'm6' : 8,   'M6': 9, 
    'm7' : 10,  'M7': 11, 
 
    '-5' : 6,   '5' : 7,    '+5' : 8,
    'b9' : 1,   '9' : 2,    '#9' : 3,
    'b11': 4,   '11': 5,    '#11': 6,
    'b13': 8,   '13': 9,

    'sus2':2,   'sus4':5
}

const _tension_index_map = {
    'P1' : 0,
    'm2' : 1,   'M2': 1,
    'm3' : 2,   'M3': 2,
    'd4' : 3,   'P4': 3,    'A4' : 3,
    'd5' : 4,   'P5': 4,    'A5' : 4,
    'm6' : 5,   'M6': 5, 
    'm7' : 6,   'M7': 6, 
 
    '-5' : 4,   '5' : 4,    '+5' : 4,
    'b9' : 1,   '9' : 1,    '#9' : 1,
    'b11': 3,   '11': 3,    '#11': 3,
    'b13': 5,   '13': 5,

    'sus2':1,   'sus4':3
}


function addSemitones(note, addIndex, addSemitones){
    let node_s = _note_name_map[note]
    let node_i = _note_index_map[note]
    let res_s = (node_s + addSemitones + 132) % 12
    let res_i = (node_i + addIndex + 77) % 7
    let res_i_s = _note_name_map[_note_index_arr[res_i]]

    let add = ''
    let s_delta = res_s - res_i_s
    s_delta = (s_delta + 126) % 12 - 6
    if (s_delta < 0) add = 'b'.repeat(-s_delta)
    else if (s_delta > 0) add = '#'.repeat(s_delta)

    return add + _note_index_arr[res_i]
}




const _reg_chord = /([b#]?(?:IV|I{1,3}|VI{0,3}))([+°ø]|)(mM|[Mm]|)(11|13|69|[12345679]|)([+-]5|)(sus2)?(sus4)?(alt)?(?:\((?:add)?([0-9,b#]+)\)|)(?:\((?:add)?([0-9,b#]+)\)|)(?:\(omit([0-9,]+)\)|)(\/[b#]?[1-7]|)/
function parseChord(str){
    let match = _reg_chord.exec(str)
    if (!match) return null

    return {
        root: match[1],
        mod: match[2] || '',
        quality: match[3] || '',
        degree: match[4] || '',
        fifth: match[5] || '',
        sus2: match[6] || '',
        sus4: match[7] || '',
        alt: match[8] || '',
        tensions: [...(match[9] || '').split(','), ...(match[10] || '').split(',')],
        omits: match[11] ? match[11].split(','): [],
        slash: match[12] || '',
    }
}

const _chord_text_map = {
    'bI'   : '降1级',  'I'   : '1级',  '#I'   : '升1级',
    'bII'  : '降2级',  'II'  : '2级',  '#II'  : '升2级',
    'bIII' : '降3级',  'III' : '3级',  '#III' : '升3级',
    'bIV'  : '降4级',  'IV'  : '4级',  '#IV'  : '升4级',
    'bV'   : '降5级',  'V'   : '5级',  '#V'   : '升5级',
    'bVI'  : '降6级',  'VI'  : '6级',  '#VI'  : '升6级',
    'bVII' : '降7级',  'VII' : '7级',  '#VII' : '升7级',
}
const _quality_text_map = {
    '+': '增',  '°': '减',  'ø': '半减七',
    'M': '大',  'm': '小',  'mM':'小大'
}
const _degree_text_map = {
    '1': '根音',    '2': '二',
    '3': '三度',    '4': '四度',
    '5': '强力',    '6': '六',
    '7': '七',      '9': '九',
    '11': '十一',   '13': '十三',   '69': '六九'
}
const _fifth_text_map = {
    '+5': '增五',   '-5': '减五'
}
const _text_map = {
    '1': '一',  '2': '二',  '3': '三',
    '4': '四',  '5': '五',  '6': '六',
    '7': '七',  '9': '九',  '11':'十一',  '13':'十三'
}
const _tune_map = {
    'b': '降', '#': '升'
}

const _reg_tension = /(#|b)?([1-79]+)/
function chordToText({root, mod, quality, degree, fifth, sus2, sus4, alt, tensions, omits, slash}){
    let s_root = _chord_text_map[root]
    let s_mod = (_quality_text_map[mod] || '') + (_quality_text_map[quality] || (mod ? '' :(+degree>=7?'属': '大')))
    let s_degree = _degree_text_map[degree] ||  (mod == 'ø' ? '' : '三')
    let s_fifth = _fifth_text_map[fifth] || ''
    let s_sus = (sus2 && '挂二' || '') + (sus4 && '挂四' || '')
    let s_alt = alt || ''
    let s_tensions = tensions ? tensions.map(e=>{
        if (!e) return ''
        let match = _reg_tension.exec(e)
        return ((match[1] && _tune_map[match[1]]) || '加') + _text_map[match[2]]
    }).join('') : ''
    let s_omits = omits ? omits.map(e=>'省' + _text_map[e]).join(''): ''

    return `${s_root}${s_mod}${s_degree}${s_fifth}${s_sus}${s_alt}${s_tensions}${s_omits}和弦${slash}`
}