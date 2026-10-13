-- Flower types, one per folder in wwwroot/images/flowers.
-- NOTE: names and prices are placeholders — replace them with the real values.
-- Registration gives new users flower types with id 8 and 9, so those ids must exist.

INSERT INTO flowertypes (id, name, code, price_buy, price_sell) VALUES
    (1,  'Алоэ медицины',                'aloe_medicine',           50, 30),
    (2,  'Кактус физики',                'cactus_physics',          50, 30),
    (3,  'Папоротник биологии',          'fern_biology',            50, 30),
    (4,  'Мандариновое дерево экологии', 'mandarin_ecology',        50, 30),
    (5,  'Лук химии',                    'onion_chemistry',         50, 30),
    (6,  'Орхидея поэзии',               'orchid_poetry',           50, 30),
    (7,  'Пальма климата',               'palm_climate',            50, 30),
    (8,  'Фикус классики',               'ficus_classic',           50, 30),
    (9,  'Зелёный хищник',               'predatory_grass',         50, 30),
    (10, 'Роза бессмертия',              'rose_eternal',            50, 30),
    (11, 'Спатифиллум реакций',          'spathiphyllum_reactions', 50, 30),
    (12, 'Фиалка волны',                 'violet_wave',             50, 30);

-- Explicit ids don't advance the sequence; move it past them.
SELECT setval('flowertypes_id_seq', (SELECT MAX(id) FROM flowertypes));

