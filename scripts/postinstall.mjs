import { cpSync, existsSync, rmSync } from 'node:fs'
import { join } from 'node:path'
import { spawnSync } from 'node:child_process'

const projectRoot = process.cwd()
const nuxtBin = join(projectRoot, 'node_modules', 'nuxt', 'bin', 'nuxt.mjs')
const isRenderBuild = process.env.RENDER === 'true'
  || Boolean(process.env.RENDER_SERVICE_ID)
  || Boolean(process.env.RENDER_EXTERNAL_URL)

runNuxt('prepare')

if (isRenderBuild) {
  runNuxt('generate')
  copyOutputToDist()
}

function runNuxt(command) {
  const result = spawnSync(process.execPath, [nuxtBin, command], {
    cwd: projectRoot,
    env: process.env,
    stdio: 'inherit'
  })

  if (result.error) {
    console.error(result.error.message)
    process.exit(1)
  }

  if (result.status !== 0) {
    process.exit(result.status ?? 1)
  }
}

function copyOutputToDist() {
  const source = join(projectRoot, '.output', 'public')
  const target = join(projectRoot, 'dist')

  if (!existsSync(source)) {
    console.error('Nuxt nao gerou .output/public.')
    process.exit(1)
  }

  rmSync(target, { recursive: true, force: true })
  cpSync(source, target, { recursive: true })
  console.log('Build estatico copiado de .output/public para dist.')
}
