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

const _note_C_map = {
    'b1' : 'Cb',  '1' : 'C',  '#1' : 'C#',
    'b2' : 'Db',  '2' : 'D',  '#2' : 'D#',
    'b3' : 'Eb',  '3' : 'E',  '#3' : 'E#',
    'b4' : 'Fb',  '4' : 'F',  '#4' : 'F#',
    'b5' : 'Gb',  '5' : 'G',  '#5' : 'G#',
    'b6' : 'Ab',  '6' : 'A',  '#6' : 'A#',
    'b7' : 'Bb',  '7' : 'B',  '#7' : 'B#',
}

const _note_abs_map = {
    'Cb': -1, 'C': 0,   'C#': 1,
    'Db': 1,  'D': 2,   'D#': 3,
    'Eb': 3,  'E': 4,   'E#': 5,
    'Fb': 4,  'F': 5,   'F#': 6,
    'Gb': 6,  'G': 7,   'G#': 8,
    'Ab': 8,  'A': 9,   'A#': 10,
    'Bb': 10, 'B': 11,  'B#': 12,
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
const _chord_root_map = {
    'bI'   : 'b1', 'I'   : '1', '#I'   : '#1',
    'bII'  : 'b2', 'II'  : '2', '#II'  : '#2',
    'bIII' : 'b3', 'III' : '3', '#III' : '#3',
    'bIV'  : 'b4', 'IV'  : '4', '#IV'  : '#4',
    'bV'   : 'b5', 'V'   : '5', '#V'   : '#5',
    'bVI'  : 'b6', 'VI'  : '6', '#VI'  : '#6',
    'bVII' : 'b7', 'VII' : '7', '#VII' : '#7',
}
const _note_name_map = {
    'b1' : 11, '1' : 0,  '#1' : 1,
    'b2' : 1,  '2' : 2,  '#2' : 3,
    'b3' : 3,  '3' : 4,  '#3' : 5,
    'b4' : 4,  '4' : 5,  '#4' : 6,
    'b5' : 6,  '5' : 7,  '#5' : 8,
    'b6' : 8,  '6' : 9,  '#6' : 10,
    'b7' : 10, '7' : 11, '#7' : 0,   '': -1
}

const _note_hi_map = {
    'b1' : -1, '1' : 0,  '#1' : 1,
    'b2' : 1,  '2' : 2,  '#2' : 3,
    'b3' : 3,  '3' : 4,  '#3' : 5,
    'b4' : 4,  '4' : 5,  '#4' : 6,
    'b5' : 6,  '5' : 7,  '#5' : 8,
    'b6' : 8,  '6' : 9,  '#6' : 10,
    'b7' : 10, '7' : 11, '#7' : 12,  '': -10
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


function addSemitones (note, add_index, add_semitones){
    let node_s = _note_name_map[note]
    let node_i = _note_index_map[note]
    let res_s = (node_s + add_semitones + 132) % 12
    let res_i = (node_i + add_index + 77) % 7
    let res_i_s = _note_name_map[_note_index_arr[res_i]]

    let add = ''
    let s_delta = res_s - res_i_s
    s_delta = (s_delta + 126) % 12 - 6

    if (s_delta <= -2) return addSemitones(note, add_index+6, add_semitones)
    if (s_delta >= 2) return addSemitones(note, add_index+1, add_semitones)

    if (s_delta < 0) add = 'b'.repeat(-s_delta)
    else if (s_delta > 0) add = '#'.repeat(s_delta)

    return add + _note_index_arr[res_i]
}

const _reg_digit = /\d+/
function addInterval(note, interval){
    let semitones = _tension_map[interval]
    let indices = _tension_index_map[interval]
    return addSemitones(note, indices, semitones)
}



const _reg_chord = /([b#]?(?:IV|I{1,3}|VI{0,3}))([+°ø]|)(mM|[Mm]|)(11|13|69|[12345679]|)([+-]5|)(sus2)?(sus4)?(alt)?(?:\((?:add)?([0-9,b#]+)\)|)(?:\((?:add)?([0-9,b#]+)\)|)(?:\(omit([0-9,]+)\)|)(?:\/([b#]?[1-7])|)/
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
        tensions: [...(match[10] || '').split(','), ...(match[9] || '').split(',')].filter(s=>s),
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

const _quality_map = {
    'M' : ['M3', 'M7'],
    'm' : ['m3', 'm7'],
    'mM': ['m3', 'M7'],
    ''  : ['M3', 'm7'],
    '+' : ['M3', 'm7'],
    'ø' : ['m3', 'm7'],
    '°' : ['m3', 'd7'],
}

const _mod_fifth_map = {
    '+': '+5', '+5': '+5',
    '°': '-5', '-5': '-5',
    'ø': '-5', ''  : '5'
}

const _degree_map = {
    ''  : [true, true , true , false],
    '1' : [true, false, false, false],
    '2' : [true, false, false, false],
    '3' : [true, true , false, false],
    '4' : [true, false, false, false],
    '5' : [true, false, true , false],
    '6' : [true, true , true , false],
    '7' : [true, true , true , true ],
    '9' : [true, true , true , true ],
    '11': [true, true , true , true ],
    '13': [true, true , true , true ],
    '69': [true, true , true , false],
}

const _degree_add_map = {
    ''  : [],
    '1' : [],
    '2' : ['9'],
    '3' : [],
    '4' : ['11'],
    '5' : [],
    '6' : ['13'],
    '7' : [],
    '9' : ['9'],
    '11': ['9', '11'],
    '13': ['9', '11', '13'],
    '69': ['9', '13'],
}

const _omit_map = {
    '1' : 0, '3': 1, '5': 2, '7': 3
}

const _reg_tension = /(#|b)?([1-79]+)/
function chordToText({root, mod, quality, degree, fifth, sus2, sus4, alt, tensions, omits, slash}){
    let s_root = _chord_text_map[root]
    let s_mod = (_quality_text_map[mod] || '') + (_quality_text_map[quality] || ((mod || degree=='69' || (+degree<=5 && degree != '')) ? '' :(+degree>=7?'属': '大')))
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
    let s_slash = slash ? '/' + slash : ''

    return `${s_root}${s_mod}${s_degree}${s_fifth}${s_sus}${s_alt}${s_tensions}${s_omits}和弦${s_slash}`
}

function toNoteAbs(note){
    let num = note.slice(-1)
    let other = note.slice(0,-1)
    return _note_abs_map[other] + num * 12
}


function getChordTones({root, mod, quality, degree, fifth, sus2, sus4, alt, tensions, omits, slash}){
    if (mod=='ø' && degree=='') degree='7'

    root = _chord_root_map[root]
    let raw_fifth = _mod_fifth_map[mod]
    let q_37 = _quality_map[quality || mod]

    let deg_t = ['P1', q_37[0], raw_fifth, q_37[1]]
    let deg_b = [..._degree_map[degree]]
    if (sus2 || sus4) deg_b[1] = false
    if (fifth) deg_b[2] = false

    let adds = [..._degree_add_map[degree]]
    adds.push(...tensions)

    omits.forEach(o => {
        deg_b[_omit_map[o]] = false
    })

    let tones = []
    if(deg_b[0]) tones.push(deg_t[0])
    if(deg_b[1]) tones.push(deg_t[1])
    if(sus2) tones.push(sus2)
    if(sus4) tones.push(sus4)
    if(deg_b[2]) tones.push(deg_t[2])
    if(fifth) tones.push(fifth)
    if(deg_b[3]) tones.push(deg_t[3])
    tones.push(...adds)

    let bass = slash || root
    let notes = tones.map(t=>addInterval(root, t))
    let fifth_note = addInterval(root, fifth || deg_t[2])
    let third_note = addInterval(root, deg_t[1])
    let highest_note = _tension_map[fifth] > 6 ? (deg_b[3] && deg_b[1] ? third_note: root): fifth_note
    let sec_note = _tension_map[fifth] > 6 ? fifth_note: (deg_b[3] && deg_b[1]  ? third_note: root)
    let plays = []

    lastNote = ''
    let octave = 5
    for (let n of notes){//.filter(n => n!=bass)){
        if(_note_hi_map[lastNote] > _note_hi_map[n] && octave <= 5){
            octave++
        }

        plays.push(`${_note_C_map[n]}${octave}`)
        lastNote = n
    }
    if(plays.length < 6) plays.push(`${_note_C_map[highest_note]}6`)
    if(plays.length < 6) plays.push(`${_note_C_map[sec_note]}6`)
    if(plays.length < 6) plays.push(`${_note_C_map[highest_note]}5`)


    return {
        bass,
        notes,
        playingKeys: {
            bass: `${_note_C_map[bass]}4`,
            notes: [...new Set(plays)].sort((a,b)=>toNoteAbs(a)-toNoteAbs(b)),
        }
    }
}

function toKeyC(note){
    return _note_C_map[note]
}