-- =========================================
-- 1. Condições dos jogos
-- =========================================

USE retrogames;

INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo)
VALUES
    (1, 'Novo'),
    (2, 'Usado'),
    (3, 'Recondicionado');


-- =========================================
-- 2. Perfis de usuários
-- =========================================

INSERT INTO tb_perfil (co_perfil, ds_perfil)
VALUES
    (1, 'Administrador'),
    (2, 'Cliente');


-- =========================================
-- 3. Plataformas
-- =========================================

INSERT INTO tb_plataforma
    (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES
    ('Atari 2600', 'Atari', 1977),
    ('Nintendo Entertainment System (NES)', 'Nintendo', 1983),
    ('Super Nintendo Entertainment System (SNES)', 'Nintendo', 1990),
    ('Sega Mega Drive', 'Sega', 1988),
    ('Sony PlayStation', 'Sony', 1994),
    ('Nintendo 64', 'Nintendo', 1996);


-- =========================================
-- 4. Gêneros
-- =========================================

INSERT INTO tb_genero (no_genero)
VALUES
    ('Ação'),
    ('Aventura'),
    ('RPG'),
    ('Plataforma'),
    ('Luta'),
    ('Corrida'),
    ('Esporte'),
    ('Tiro');

-- =========================================
-- 5. Jogos
-- =========================================

INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
     qt_estoque, st_ativo, dt_cadastro,
     co_plataforma, co_condicao_jogo)
VALUES
    ('Adventure', 'Aventura clássica do Atari 2600.', 1979, 49.90, 5, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Atari 2600'), 2),

    ('Pitfall!', 'Explore cenários repletos de obstáculos e tesouros.', 1982, 59.90, 3, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Atari 2600'), 2),

    ('Super Mario Bros.', 'Clássico jogo de plataforma da Nintendo.', 1985, 89.90, 8, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Nintendo Entertainment System (NES)'), 2),

    ('The Legend of Zelda', 'Aventura de exploração no reino de Hyrule.', 1986, 99.90, 4, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Nintendo Entertainment System (NES)'), 2),

    ('Metroid', 'Aventura de ação em um planeta alienígena.', 1986, 79.90, 6, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Nintendo Entertainment System (NES)'), 2),

    ('Mega Man 2', 'Enfrente robôs e desafios em fases variadas.', 1988, 89.90, 2, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Nintendo Entertainment System (NES)'), 2),

    ('Contra', 'Jogo de ação e tiro cooperativo.', 1987, 69.90, 5, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Nintendo Entertainment System (NES)'), 2),

    ('Super Metroid', 'Explore um mundo alienígena em uma aventura de ação.', 1994, 149.90, 3, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Super Nintendo Entertainment System (SNES)'), 2),

    ('The Legend of Zelda: A Link to the Past', 'Aventura épica no universo de Zelda.', 1991, 139.90, 4, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Super Nintendo Entertainment System (SNES)'), 2),

    ('Super Mario World', 'Aventura de plataforma com Mario e Yoshi.', 1990, 119.90, 7, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Super Nintendo Entertainment System (SNES)'), 2),

    ('Donkey Kong Country', 'Aventura de plataforma na ilha dos Kongs.', 1994, 129.90, 3, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Super Nintendo Entertainment System (SNES)'), 2),

    ('Street Fighter II', 'Clássico jogo de luta competitivo.', 1991, 99.90, 5, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Super Nintendo Entertainment System (SNES)'), 2),

    ('Sonic the Hedgehog', 'Aventura de plataforma com o famoso ouriço azul.', 1991, 89.90, 6, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Sega Mega Drive'), 2),

    ('Streets of Rage 2', 'Jogo de ação e combate nas ruas.', 1992, 109.90, 4, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Sega Mega Drive'), 2),

    ('Phantasy Star IV', 'RPG ambientado em um universo de ficção científica.', 1993, 159.90, 2, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Sega Mega Drive'), 2),

    ('Mortal Kombat', 'Jogo de luta com combates entre personagens.', 1992, 79.90, 0, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Sega Mega Drive'), 2),

    ('Castlevania: Symphony of the Night', 'Aventura de ação com elementos de RPG.', 1997, 199.90, 3, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Sony PlayStation'), 2),

    ('Final Fantasy VII', 'RPG clássico ambientado em um mundo futurista.', 1997, 189.90, 0, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Sony PlayStation'), 2),

    ('Super Mario 64', 'Aventura de plataforma em ambientes tridimensionais.', 1996, 179.90, 4, TRUE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Nintendo 64'), 2),

    ('Mario Kart 64', 'Jogo de corrida com personagens da Nintendo.', 1996, 149.90, 5, FALSE, UTC_TIMESTAMP(),
     (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'Nintendo 64'), 2);

-- =========================================
-- 6. Relacionamento entre jogos e gêneros
-- =========================================

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero IN ('Aventura', 'Ação')
WHERE j.no_jogo = 'Adventure';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero IN ('Plataforma', 'Aventura')
WHERE j.no_jogo = 'Pitfall!';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero = 'Plataforma'
WHERE j.no_jogo = 'Super Mario Bros.';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero IN ('Aventura', 'RPG')
WHERE j.no_jogo = 'The Legend of Zelda';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero IN ('Ação', 'Aventura')
WHERE j.no_jogo = 'Metroid';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero IN ('Ação', 'Plataforma')
WHERE j.no_jogo = 'Mega Man 2';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero IN ('Ação', 'Tiro')
WHERE j.no_jogo = 'Contra';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero IN ('Ação', 'Aventura')
WHERE j.no_jogo = 'Super Metroid';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero IN ('Aventura', 'RPG')
WHERE j.no_jogo = 'The Legend of Zelda: A Link to the Past';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero = 'Plataforma'
WHERE j.no_jogo = 'Super Mario World';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero IN ('Plataforma', 'Aventura')
WHERE j.no_jogo = 'Donkey Kong Country';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero = 'Luta'
WHERE j.no_jogo = 'Street Fighter II';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero = 'Plataforma'
WHERE j.no_jogo = 'Sonic the Hedgehog';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero IN ('Ação', 'Luta')
WHERE j.no_jogo = 'Streets of Rage 2';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero = 'RPG'
WHERE j.no_jogo = 'Phantasy Star IV';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero = 'Luta'
WHERE j.no_jogo = 'Mortal Kombat';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero IN ('Ação', 'Aventura', 'RPG')
WHERE j.no_jogo = 'Castlevania: Symphony of the Night';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero = 'RPG'
WHERE j.no_jogo = 'Final Fantasy VII';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero IN ('Plataforma', 'Aventura')
WHERE j.no_jogo = 'Super Mario 64';

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM tb_jogo j
JOIN tb_genero g ON g.no_genero = 'Corrida'
WHERE j.no_jogo = 'Mario Kart 64';

-- =========================================
-- 7. Consultas de validação
-- =========================================

-- Quantidade de plataformas
SELECT COUNT(*) AS total_plataformas FROM tb_plataforma;

-- Quantidade de gêneros
SELECT COUNT(*) AS total_generos FROM tb_genero;

-- Quantidade de jogos
SELECT COUNT(*) AS total_jogos FROM tb_jogo;

-- Jogos sem nenhum gênero (resultado esperado: zero linhas)
SELECT
    j.co_seq_jogo,
    j.no_jogo
FROM tb_jogo j
LEFT JOIN tb_jogo_genero jg
    ON jg.co_jogo = j.co_seq_jogo
WHERE jg.co_jogo IS NULL;

-- Jogos inativos (deve retornar pelo menos um)
SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE st_ativo = FALSE;

-- Jogos sem estoque (deve retornar pelo menos um)
SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE qt_estoque = 0;

-- Quantidade de usuários (resultado esperado: zero)
SELECT COUNT(*) AS total_usuarios FROM tb_usuario;