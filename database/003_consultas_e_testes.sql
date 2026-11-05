USE retrogames;

-- 1. Catálogo: jogos ativos com plataforma e condição
SELECT
    j.no_jogo,
    p.no_plataforma,
    c.ds_condicao_jogo,
    j.vl_preco,
    j.qt_estoque
FROM tb_jogo j
INNER JOIN tb_plataforma p
    ON j.co_plataforma = p.co_seq_plataforma
INNER JOIN tb_condicao_jogo c
    ON j.co_condicao_jogo = c.co_condicao_jogo
WHERE j.st_ativo = TRUE
ORDER BY j.no_jogo;


-- 2. Gêneros de um jogo informado
SET @nome_jogo = 'Super Mario 64';

SELECT
    j.no_jogo,
    g.no_genero
FROM tb_jogo j
INNER JOIN tb_jogo_genero jg
    ON j.co_seq_jogo = jg.co_jogo
INNER JOIN tb_genero g
    ON jg.co_genero = g.co_seq_genero
WHERE j.no_jogo = @nome_jogo;


-- 3. Jogos de um gênero informado
SET @nome_genero = 'Ação';

SELECT
    j.no_jogo,
    j.nu_ano_lancamento,
    p.no_plataforma
FROM tb_jogo j
INNER JOIN tb_jogo_genero jg
    ON j.co_seq_jogo = jg.co_jogo
INNER JOIN tb_genero g
    ON jg.co_genero = g.co_seq_genero
INNER JOIN tb_plataforma p
    ON j.co_plataforma = p.co_seq_plataforma
WHERE g.no_genero = @nome_genero
ORDER BY j.no_jogo;


-- 4. Quantidade de jogos por plataforma, incluindo plataformas sem jogos
SELECT
    p.no_plataforma,
    COUNT(j.co_seq_jogo) AS quantidade_jogos
FROM tb_plataforma p
LEFT JOIN tb_jogo j
    ON p.co_seq_plataforma = j.co_plataforma
GROUP BY p.co_seq_plataforma, p.no_plataforma
ORDER BY p.no_plataforma;


-- 5. Busca por parte do nome, com 5 itens por página
SET @termo_nome = 'Mario';

SELECT
    j.no_jogo,
    p.no_plataforma,
    j.nu_ano_lancamento
FROM tb_jogo j
INNER JOIN tb_plataforma p
    ON j.co_plataforma = p.co_seq_plataforma
WHERE j.no_jogo LIKE CONCAT('%', @termo_nome, '%')
ORDER BY j.no_jogo
LIMIT 5 OFFSET 0;